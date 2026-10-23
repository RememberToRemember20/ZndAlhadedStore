using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Command
{
    public record CreateQuoteCommand : IRequest<GetQuoteDTO>
    {
        public int ClientId { get; init; }
        public decimal ExchangeRate { get; init; }
        public decimal Discount { get; init; }
        public ICollection<CreateQuoteItemDTO> QuoteItems { get; init; } = new List<CreateQuoteItemDTO>();
    }
}
