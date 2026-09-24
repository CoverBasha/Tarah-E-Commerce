using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.API.Models.DTOs;
using Tarah.API.Services;

namespace Tarah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService service;

        public ProductsController(ProductsService service)
        {
            this.service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> All([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var response = await service.AllProductsAsync(pageNumber ?? 1, pageSize ?? 10);

            return Ok(response.Result);
        }



        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await service.ProductByIdAsync(id);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            return Ok(response.Result);
        }




        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await service.AddProductAsync(dto, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Error)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetById), new { id = response.Result.Id }, response.Result);

        }




        [HttpPut]
        [Authorize]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct([FromRoute]Guid id, [FromForm]UpdateProductDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await service.UpdateProductAsync(id, dto, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if(response.Status == Status.Error)
                return BadRequest(response.Message);

            if (response.Status == Status.Forbidden)
                return Forbid(response.Message);

            return Ok(response.Result);
        }



        [HttpDelete]
        [Authorize]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct([FromRoute]Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await service.DeleteProductAsync(id, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Forbidden)
                return Forbid(response.Message);

            return Ok();
        }

    }
}
