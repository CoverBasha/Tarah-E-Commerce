using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.Identity.Models;

namespace Tarah.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> UserById(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = id,
                Username = user.UserName,
            };

            return Ok(userDto);
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var profileDto = new ProfileDto
            {
                Id = Guid.Parse(id),
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(profileDto);
        }
    }
}
