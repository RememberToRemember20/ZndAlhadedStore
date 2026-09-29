using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.ProductCommandHandler.Command;

namespace ZndAlhadedStore.ProductCommandHandler.Handler
{
    public class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand, string>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileStorageService _storageService;
        public UpdateProductImageCommandHandler(AppDbContext dbContext, IFileStorageService storageService)
        {
            _appDbContext = dbContext;
            _storageService = storageService;
        }
        public async Task<string> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            var prod = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (prod == null)
            { return null; }
            else
            {
                    _storageService.DeleteFile(prod.ImageURL);
                 prod.ImageURL = await _storageService.SaveFileAsync(request.Image);
                    _appDbContext.Products.Update(prod);
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                    return prod.ImageURL;
               
            }
        }
    }
}
