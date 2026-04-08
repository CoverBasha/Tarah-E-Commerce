using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.API.Services;

namespace Tarah.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly CartsService service;

        public CartsController(CartsService service)
        {
            this.service = service;
        }


        [HttpGet]
        [Route("Cart")]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await service.GetCartDto(userId);

            return Ok(response.Result);
        }

        [HttpPost]
        [Authorize]
        [Route("{productId:guid}")]
        public async Task<IActionResult> AddToCart([FromRoute]Guid productId,[FromBody]int quantity)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await service.AddToCart(productId, quantity, userId);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);
            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            return Ok(response.Result);
        }


        [HttpPut]
        [Authorize]
        [Route("Modify/{productId:guid}")]
        public async Task<IActionResult> ModifyItemCount([FromRoute]Guid productId,[FromBody]int quantity)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await service.ModifyProductCount(productId, quantity, userId);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);
            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            return Ok(response.Result);
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete/{productId:guid}")]
        public async Task<IActionResult> DeleteItem([FromRoute]Guid productId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await service.RemoveFromCart(userId, productId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            return Ok(response.Result);
        }

        [HttpDelete]
        [Authorize]
        [Route("Clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await service.ClearCart(userId);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            return Ok(response.Result);
        }

        [HttpGet]
        [Authorize]
        [Route("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await service.Checkout(userId);

            if (response.Status == Status.Forbidden)
                return BadRequest(response.Message);
            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            return Ok(response.Result);
        }

    }
}
