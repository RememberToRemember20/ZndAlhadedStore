using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Query
{
    public record GetAllQuoteQuery : IRequest<IEnumerable<GetQuoteDTO>>;
    
}
