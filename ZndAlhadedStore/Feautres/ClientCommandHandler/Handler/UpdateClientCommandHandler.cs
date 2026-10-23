using AutoMapper;
using MediatR;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Command;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Handler
{
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, GetClientDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public UpdateClientCommandHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<GetClientDTO> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _context.Clients.FindAsync(request.Id);
            if (client == null)
            {
                return null;
            }
            _mapper.Map(request, client);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<GetClientDTO>(client);
        }

    }
}
