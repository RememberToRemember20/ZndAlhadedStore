using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Command
{
    public record CreateClientCommand(string Name, string PhoneNumber) : IRequest<GetClientDTO>;
}
