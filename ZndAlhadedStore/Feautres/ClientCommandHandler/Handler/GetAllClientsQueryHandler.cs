using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.ClientCommandHandler.Handler
{
    public class GetAllClientsQueryHandler:IRequestHandler<GetAllClientsQuery,IList<GetClientDTO>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appContext;
        public GetAllClientsQueryHandler(IMapper mapper, AppDbContext appContext)
        {
            _mapper = mapper;
            _appContext = appContext;
        }
        public async Task<IList<GetClientDTO>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _appContext.Clients.ToListAsync(cancellationToken);
            return _mapper.Map<IList<GetClientDTO>>(clients);
        }
    }
}
