using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.ProductCommandHandler.Command
{
    public record UpdateProductCommand:IRequest<GetProductDTO>
    {
        public int Id { get; set; }
        public string? Name { get; init; }
        public string? ArabicDescription { get; init; }
        public string? EnglishDescription { get; init; }
        public decimal? Price { get; init; }
        public Currency? Currency { get; init; }
    }
}
