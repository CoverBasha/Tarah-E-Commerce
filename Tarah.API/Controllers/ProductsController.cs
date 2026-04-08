using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.API.Models.DTOs;
using Tarah.API.Service;
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
        public async Task<IActionResult> All(Guid? categoryId,int? pageNumber, int? pageSize)
        {
            var response = await service.AllProductsAsync(categoryId, pageNumber ?? 1, pageSize ?? 10);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);

            if (response.Status == Status.Unauthorized)
                return Forbid(response.Message);

            return Ok(response.Result);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await service.ProductByIdAsync(id);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);

            if (response.Status == Status.Unauthorized)
                return Forbid(response.Message);

            return Ok(response.Result);
        }

        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await service.AddProductAsync(dto, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);

            if (response.Status == Status.Unauthorized)
                return Forbid(response.Message);

            return CreatedAtAction(nameof(GetById), new { id = response.Result.Id }, response.Result);

        }

        [HttpPut]
        [Authorize]
        [Route("Update/{id:guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute]Guid id, [FromForm]UpdateProductDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await service.UpdateProductAsync(id, dto, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);

            if (response.Status == Status.Unauthorized)
                return Forbid(response.Message);

            return Ok(response.Result);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute]Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await service.DeleteProductAsync(id, userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            
            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);
            
            if (response.Status == Status.Unauthorized)
                return Forbid(response.Message);

            return Ok(response.Result);
        }

    }
}
