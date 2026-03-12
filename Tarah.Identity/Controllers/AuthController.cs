using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tarah.Identity.Models;
using Tarah.Identity.Services;

namespace Tarah.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly AuthService service;

        public AuthController(UserManager<IdentityUser> userManager, AuthService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            var exists = await userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
                return BadRequest("A User with this email exists");

            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if(result.Succeeded)
            {
                _ = Task.Run(async () => await service.PublishUser(user));
                return Ok("Registered Successfully");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Incorrect Email");

            var isMatching = await userManager.CheckPasswordAsync(user, dto.Password);

            if(isMatching)
            {
                var token = service.CreateToken(user);
                return Ok(token);
            }
            return BadRequest("Incorrect Password");
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound("User not found");

            var deleterResult = await userManager.DeleteAsync(user);

            if (deleterResult.Succeeded)
            {
                await service.DeleteUser(user);
                return Ok("User deleted Successfully");
            }

            return BadRequest(deleterResult.Errors);
        }
    }
}
