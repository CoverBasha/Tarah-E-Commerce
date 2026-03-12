using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using MassTransit;
using Tarah.Contracts.Events;

namespace Tarah.Identity.Services
{
    public class AuthService
    {
        private readonly IConfiguration config;
        private readonly IPublishEndpoint publishEndpoint;

        public AuthService(IConfiguration config, IPublishEndpoint publishEndpoint)
        {
            this.config = config;
            this.publishEndpoint = publishEndpoint;
        }
        public string CreateToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1d),
                signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task PublishUser(IdentityUser user)
        {
            var userId = Guid.Parse(user.Id);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            await publishEndpoint.Publish(new UserCreated(userId), cts.Token);
        }
        
        public async Task DeleteUser(IdentityUser user)
        {
            var userId = Guid.Parse(user.Id);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            await publishEndpoint.Publish(new UserDeleted(userId),cts.Token);
        }
    }
}
