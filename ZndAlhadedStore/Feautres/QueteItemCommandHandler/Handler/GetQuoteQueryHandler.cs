using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Handler
{
    public class GetQuoteQueryHandler:IRequestHandler<GetQuoteQuery, GetQuoteDTO>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        
        public GetQuoteQueryHandler(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<GetQuoteDTO> Handle(GetQuoteQuery request, CancellationToken cancellationToken)
        {
            var quote = await _context.Quotes.FindAsync(request.Id, cancellationToken);
            if (quote == null)
            {
                throw new Exception("Quote not found");
            }
            return _mapper.Map<GetQuoteDTO>(quote);
        }
    }
}
