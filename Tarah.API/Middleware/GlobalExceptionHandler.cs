using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using Tarah.API.Data;

namespace Tarah.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new { ex.Message });
            }
        }
    }
}
