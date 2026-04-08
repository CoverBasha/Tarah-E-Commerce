using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tarah.API.Data;

namespace Tarah.API.Middleware
{
    public class CheckUserExists
    {
        private readonly RequestDelegate next;

        public CheckUserExists(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, TarahDbContext dbContext)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!string.IsNullOrEmpty(userId))
            {
                var guid = Guid.Parse(userId);
                var user = await dbContext.DeletedUsers.SingleOrDefaultAsync(u => u.UserId == guid);

                if (user != null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("User no longer exists");
                    return;
                }
            }

            await next(context);
        }
    }
}
