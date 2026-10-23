using AutoMapper;
using MediatR;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Command;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Handler
{
    public class DeleteClientCommandHandler:IRequestHandler<DeleteClientCommand, GetClientDTO>
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public DeleteClientCommandHandler(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }
        public async Task<GetClientDTO> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _appContext.Clients.FindAsync(request.Id);
            if (client == null)
            {
                return null;
            }
            _appContext.Clients.Remove(client);
            await _appContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GetClientDTO>(client);
        }
    }
}
