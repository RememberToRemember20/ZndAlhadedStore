using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Query
{
    public record GetClientQuery(int Id) : IRequest<GetClientDTO>;
}
