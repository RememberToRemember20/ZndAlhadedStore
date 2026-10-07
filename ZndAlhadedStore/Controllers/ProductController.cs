using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Shared.DTOs;
using ZndAlhadedStore.Feautres.ProductCommandHandler.Command;
using ZndAlhadedStore.ProductCommandHandler.Command;
using ZndAlhadedStore.ProductCommandHandler.Query;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }
        [HttpPost("PostProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO product, IFormFile image)
        {
            var command = _mapper.Map<CreateProductCommand>(product); // AutoMapper بيحوّل الحقول المشتركة
            command.Image = image; // الملف بنحطه يدوي لأنه مش موجود في الـ DTO أصلاً

            var newProductId = await _mediator.Send(command);
            return Ok(newProductId);
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts() 
        {
            var query=new GetAllProductsQuery();
            var products = await _mediator.Send(query);
            if (products != null)
            {
                return Ok(products);
            }
            else { return BadRequest(); }
        }
        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var query = new GetProductByIdQuery(id);
            var products = await _mediator.Send(query);
            if (products != null)
            {
                return Ok(products);
            }
            else { return BadRequest(); }
        }
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var delete = new DeleteProductByIdCommand(id);
            var deleted = await _mediator.Send(delete);
            if (deleted != null)
            {
                return Ok(deleted);
            }
            else { return NotFound(); }
        }
        [HttpPut("{id}/UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id,[FromBody] UpdateProductDTO product)
        {
            var command = _mapper.Map<UpdateProductCommand>(product);
            command.Id = id;
            var newProductId = await _mediator.Send(command);
            return Ok(newProductId);
        }
        [HttpPut("DeleteImage/{id}")]
        public async Task<IActionResult>DeleteImage(int id)
        {
            var rquest=new DeleteImageCommand(id);
            var url = await _mediator.Send(rquest);
            if (url != null) {  return Ok(url); }
            else { return NotFound(); }
        }
        [HttpPut("UpdateImage/{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromForm]IFormFile image)
        {
            var rquest = new UpdateProductImageCommand(id,image);
            var url = await _mediator.Send(rquest);
            if (url != null) { return Ok(url); }
            else { return NotFound(); }
        }
    }
}
