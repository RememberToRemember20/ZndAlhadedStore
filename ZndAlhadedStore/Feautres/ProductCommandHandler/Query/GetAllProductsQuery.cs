using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Query
{
    public class GetAllProductsQuery:IRequest<List<GetProductDTO>>
    {
    }
}
