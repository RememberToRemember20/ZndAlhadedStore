using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Entity;
using ZndAlhadedStore.Exceptions;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Command;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Handler
{
    public class CreateQuoteCommandHandler : IRequestHandler<CreateQuoteCommand, GetQuoteDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CreateQuoteCommandHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetQuoteDTO> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
        {
            if (!await _context.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken))
                throw new NotFoundException("العميل غير موجود");

            if (request.QuoteItems.Count == 0)
                throw new BusinessRuleException("عرض السعر لازم يحتوي على منتج واحد على الأقل");

            var productIds = request.QuoteItems.Select(i => i.ProductId).Distinct().ToList();
            if (productIds.Count != request.QuoteItems.Count)
                throw new BusinessRuleException("نفس المنتج مكرر في العرض");

            // استعلام واحد لكل المنتجات
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var missing = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missing.Count > 0)
                throw new NotFoundException($"منتجات غير موجودة: {string.Join(", ", missing)}");

            var quoteItems = new List<QuoteItem>();
            foreach (var item in request.QuoteItems)
            {
                var product = products[item.ProductId];
                var (priceSYP, priceUSD) = ToBothCurrencies(product, request.ExchangeRate);

                quoteItems.Add(new QuoteItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPriceSYP = priceSYP,
                    ProductPriceUSD = priceUSD,
                    ProductCurrency = product.Currency,
                    Count = item.Count,
                    Discount = item.Discount
                });
            }

            // الإجمالي من الأرقام المقرّبة المخزّنة، عشان يطابق الأسطر
            var subtotalSYP = quoteItems.Sum(i => LineTotal(i.ProductPriceSYP, i));
            var subtotalUSD = quoteItems.Sum(i => LineTotal(i.ProductPriceUSD, i));

            var quote = new Quote
            {
                ClientId = request.ClientId,
                ExchangeRate = request.ExchangeRate,
                Discount = request.Discount,
                CreatedAt = DateTime.UtcNow,
                TotalQuantity = quoteItems.Sum(i => i.Count),
                TotalSYP = Round2(subtotalSYP * (1 - request.Discount / 100m)),
                TotalUSD = Round2(subtotalUSD * (1 - request.Discount / 100m)),
                QuoteItems = quoteItems
            };

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GetQuoteDTO>(quote);
        }

        private static (decimal SYP, decimal USD) ToBothCurrencies(Product product, decimal rate) =>
            product.Currency == Currency.SYP
                ? (product.Price, Round2(product.Price / rate))
                : (Round2(product.Price * rate), product.Price);

        private static decimal LineTotal(decimal unitPrice, QuoteItem item) =>
            Round2(unitPrice * item.Count * (1 - item.Discount / 100m));

        private static decimal Round2(decimal value) =>
            Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
