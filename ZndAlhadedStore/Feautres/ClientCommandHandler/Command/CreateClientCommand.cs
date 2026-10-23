using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Command
{
    public record CreateClientCommand:IRequest<GetClientDTO>
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
