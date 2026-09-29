using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.ProductCommandHandler.Command;

namespace ZndAlhadedStore.ProductCommandHandler.Handler
{
    public class DeleteProductByIdCommandHandler:IRequestHandler<DeleteProductByIdCommand, GetProductDTO>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;
        public DeleteProductByIdCommandHandler(IFileStorageService fileStorageService,IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }
     

        public async Task<GetProductDTO> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _fileStorageService.DeleteFile(product.ImageURL);
                var map = _mapper.Map<GetProductDTO>(product);
                return map;
            }
            else { return null; }
        }
    }
}
