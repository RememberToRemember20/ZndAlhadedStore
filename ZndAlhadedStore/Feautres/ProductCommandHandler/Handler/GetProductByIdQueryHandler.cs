using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.ProductCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Handler
{
    public class GetProductByIdQueryHandler:IRequestHandler<GetProductByIdQuery,GetProductDTO>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        public GetProductByIdQueryHandler(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<GetProductDTO> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
            var map = _mapper.Map<GetProductDTO>(result);
            return map;
        }
    }
}
