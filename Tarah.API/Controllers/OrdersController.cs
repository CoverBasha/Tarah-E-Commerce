using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.API.Services;

namespace Tarah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService service;

        public OrdersController(OrdersService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AllOrders(int? pageNumber, int? pageSize)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await service.AllOrders(userId, pageNumber ?? 1, pageSize ?? 10);

            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [Route("{orderId:guid}")]
        public async Task<IActionResult> OrderById(Guid orderId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await service.OrderById(userId, orderId);

            if (response.Status == Status.Success)
                return Ok(response.Result);
            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            return BadRequest(response.Message);
        }
    }
}
