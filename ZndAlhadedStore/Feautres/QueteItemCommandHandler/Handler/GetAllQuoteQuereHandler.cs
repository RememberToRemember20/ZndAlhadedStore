using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Handler
{
    public class GetAllQuoteQuereHandler:IRequestHandler<GetAllQuoteQuery, IEnumerable<GetQuoteDTO>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public GetAllQuoteQuereHandler(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<GetQuoteDTO>> Handle(GetAllQuoteQuery request, CancellationToken cancellationToken)
        {
            var quotes = await _context.Quotes.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GetQuoteDTO>>(quotes);
        }
    }
}
