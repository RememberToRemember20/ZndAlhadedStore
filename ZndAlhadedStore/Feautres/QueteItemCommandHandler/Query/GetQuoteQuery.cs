using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.QueteItemCommandHandler.Query
{
    public record GetQuoteQuery(int Id):IRequest<GetQuoteDTO>;
   
}
