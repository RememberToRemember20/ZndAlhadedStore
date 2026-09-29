using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.ProductCommandHandler.Query
{
    public record GetProductByIdQuery:IRequest<GetProductDTO>
    {
        public int Id { get; init; }
       public GetProductByIdQuery(int id) { Id = id; }
    }
}
