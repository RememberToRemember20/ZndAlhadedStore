using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.ProductCommandHandler.Query;

namespace ZndAlhadedStore.ProductCommandHandler.Handler
{
    public class GetAllProductQueryHandler:IRequestHandler<GetAllProductsQuery,List<GetProductDTO>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        public GetAllProductQueryHandler(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<List<GetProductDTO>> Handle(GetAllProductsQuery query,CancellationToken cancellationToken)
        {
            var result= await _dbContext.Products.ToListAsync(cancellationToken);
            var map=_mapper.Map<List<GetProductDTO>>(result);
            return map;
        }
    }
}
