using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.ProductCommandHandler.Query
{
    public class GetAllProductsQuery:IRequest<List<GetProductDTO>>
    {
    }
}
