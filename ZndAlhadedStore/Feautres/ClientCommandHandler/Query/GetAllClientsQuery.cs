using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Query
{
    public record GetAllClientsQuery : IRequest<IList<GetClientDTO>>;
}
