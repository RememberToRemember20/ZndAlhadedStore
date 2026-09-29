using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Shared.DTOs;
using System.Runtime.CompilerServices;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.ProductCommandHandler.Command;

namespace ZndAlhadedStore.ProductCommandHandler.Handler
{
    public class UpdateProductCommandHandler:IRequestHandler<UpdateProductCommand,GetProductDTO>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        public UpdateProductCommandHandler(IMapper mapper,AppDbContext dbContext)
        {
            _mapper= mapper;
            _appDbContext= dbContext;
        }
        public async Task<GetProductDTO> Handle(UpdateProductCommand updateProduct, CancellationToken cancellationToken)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == updateProduct.Id, cancellationToken);
            if (product == null)
            { return null; }
            else
            {
                var result = _mapper.Map(updateProduct,product);
                _appDbContext.Products.Update(result);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                var map = _mapper.Map<GetProductDTO>(result);
                return map;
            }
        }
    }
}
