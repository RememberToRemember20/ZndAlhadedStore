using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Command;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Query;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public ClientController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }
        [HttpGet("GetClientById/{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _mediator.Send(new GetClientQuery ( id ));
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }
        [HttpGet("GetAllClients")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _mediator.Send(new GetAllClientsQuery());
            return Ok(clients);
        }
        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDTO clientDto)
        {
            var client = await _mediator.Send(clientDto);
            return Ok(client);
        }
        [HttpPut("{id}/UpdateClient")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientCommand clientDto)
        {
            var Command=_mapper.Map<UpdateClientCommand>(clientDto);
            Command.Id = id;
            var client = await _mediator.Send(Command);
            return Ok(client);
        }
        [HttpDelete("DeleteClient/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
           var delete= await _mediator.Send(new DeleteClientCommand(id));
            if (id <= 0)
            {
                return BadRequest();
            }
            if (delete == null)
            {
                return NotFound();
            }
            return Ok(delete);
        }
    }
}
