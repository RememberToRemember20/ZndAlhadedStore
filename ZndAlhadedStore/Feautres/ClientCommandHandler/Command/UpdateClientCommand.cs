using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Command
{
    public class UpdateClientCommand:IRequest<GetClientDTO>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
