using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Command
{
    public record DeleteProductByIdCommand:IRequest<GetProductDTO>
    {
        public int Id { get; set; }
        public DeleteProductByIdCommand(int id) {Id = id; }
    }
}
