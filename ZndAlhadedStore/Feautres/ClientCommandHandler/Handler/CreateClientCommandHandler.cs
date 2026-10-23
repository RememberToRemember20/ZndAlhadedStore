using AutoMapper;
using MediatR;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Entity;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Command;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Handler
{
    public class CreateClientCommandHandler:IRequestHandler<CreateClientCommand, GetClientDTO>
    {
        public readonly AppDbContext _appContext;
        public readonly IMapper _mapper;
        public CreateClientCommandHandler(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }
        public async Task<GetClientDTO> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = _mapper.Map<Client>(request);
            _appContext.Clients.Add(client);
            await _appContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GetClientDTO>(client);
        }
    }
}
