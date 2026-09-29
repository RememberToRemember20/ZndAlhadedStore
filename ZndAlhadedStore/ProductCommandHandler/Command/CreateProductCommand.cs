using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.ProductCommandHandler.Command
{
    public record CreateProductCommand:IRequest<GetProductDTO>
    {
        public string Name { get; init; }
        public string? ArabicDescription { get; init; }
        public string? EnglishDescription { get; init; }

        public decimal Price { get; init; }

        public Currency Currency { get; init; }

        public IFormFile Image { get; set; }
    }
}
