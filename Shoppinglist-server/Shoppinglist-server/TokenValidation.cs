using Microsoft.EntityFrameworkCore;

namespace Shoppinglist_server
{
    public class TokenValidation
    {
        private readonly RequestDelegate _next;

        public TokenValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ShoppingListDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var tokenEntry = await dbContext.TokenBlacklist
                .FirstOrDefaultAsync(t => t.Token == token);


            if (tokenEntry != null && (tokenEntry.IsBlacklisted || tokenEntry.Expiration < DateTime.UtcNow))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted or expired.");
                return;
            }

            await _next(context);
        }
    }

}
