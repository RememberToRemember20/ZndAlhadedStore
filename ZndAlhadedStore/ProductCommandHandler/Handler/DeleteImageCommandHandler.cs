using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.ProductCommandHandler.Command;

namespace ZndAlhadedStore.ProductCommandHandler.Handler
{
    public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, string>
    {
        private readonly AppDbContext _appDbContext;
        private IFileStorageService _fileStorageService;
        public DeleteImageCommandHandler(AppDbContext dbContext, IFileStorageService fileStorageService)
        {
            _appDbContext = dbContext;
            _fileStorageService = fileStorageService;
        }
        public async Task<string> Handle(DeleteImageCommand rquest, CancellationToken cancellationToken)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == rquest.Id, cancellationToken);
            if (product == null)
            {
                return null;
            }
            else
            {
                if (string.IsNullOrEmpty(product.ImageURL)) { return null; }
                else 
                {
                    string url=product.ImageURL;
                    _fileStorageService.DeleteFile(product.ImageURL);
                    product.ImageURL = string.Empty;
                    _appDbContext.Products.Update(product);
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                    return url;
                } 
            } 
        
        }
    }
}
