using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.API.Services;

namespace Tarah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IdentityService service;

        public UsersController(IdentityService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UserProfile([FromRoute] Guid id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await service.GetProfile(id, requesterId, pageNumber ?? 1, pageSize ?? 10);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);
            if (response.Status == Status.Forbidden)
                return Forbid(response.Message);
            if (response.Status == Status.Unauthorized)
                return Unauthorized(response.Message);

            return Ok(response.Result);
        }
    }
}
