using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Command
{
    public record DeleteClientCommand(int Id) : IRequest<GetClientDTO>;
}
