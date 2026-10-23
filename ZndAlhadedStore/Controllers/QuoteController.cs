using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using ZndAlhadedStore.Exceptions;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Command;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Query;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public QuoteController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpPost("CreateQuote")]
        public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteDTO dto)
        {
            var command = _mapper.Map<CreateQuoteCommand>(dto);
            try { return Ok(await _mediator.Send(command)); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (BusinessRuleException ex) { return BadRequest(ex.Message); }
        }
        [HttpGet("GetAllQuotes")]
        public async Task<IActionResult> GetAllQuotes()
        {
            var query = new GetAllQuoteQuery();
            var quotes = await _mediator.Send(query);
            return Ok(quotes);
        }
        [HttpGet("GetQuote/{id}")]
        public async Task<IActionResult> GetQuote(int id)
        {
            var query = new GetQuoteQuery  ( id );
            try
            {
                var quote = await _mediator.Send(query);
                return Ok(quote);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
