using MediatR;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Interfaces;
using Shared.DTOs;
using AutoMapper;
using ZndAlhadedStore.Entity;
using ZndAlhadedStore.Feautres.ProductCommandHandler.Command;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Handler
{
    public class CreateProductCommandHandler:IRequestHandler<CreateProductCommand,GetProductDTO>
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;
        public CreateProductCommandHandler(AppDbContext context,IFileStorageService fileStorage, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }
        public async Task<GetProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            
            var result=_mapper.Map<Product>(request);
            result.ImageURL = await _fileStorage.SaveFileAsync(request.Image);
            await _context.AddAsync(result);
            await _context.SaveChangesAsync(cancellationToken);
            var productResult = _mapper.Map<GetProductDTO>(result);
            return productResult;
        }
    }
}
