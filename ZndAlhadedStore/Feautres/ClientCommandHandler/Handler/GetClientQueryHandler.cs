using AutoMapper;
using MediatR;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Handler
{
    public class GetClientQueryHandler:IRequestHandler<GetClientQuery,GetClientDTO>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appContext;
        public GetClientQueryHandler(IMapper mapper, AppDbContext appContext)
        {
            _mapper = mapper;
            _appContext = appContext;
        }
        public async Task<GetClientDTO> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            var client = await _appContext.Clients.FindAsync(request.Id,cancellationToken);
            if (client == null)
            {
                return null;
            }
            return _mapper.Map<GetClientDTO>(client);
        }
    }
}
