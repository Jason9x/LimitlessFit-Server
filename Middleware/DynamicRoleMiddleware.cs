using System.Security.Claims;
using LimitlessFit.Interfaces;

namespace LimitlessFit.Middleware
{
    public class DynamicRoleMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await next(context);
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                await next(context);
                return;
            }

            using (var scope = serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var role = await userService.GetUserRoleByIdAsync(int.Parse(userId));

                if (string.IsNullOrEmpty(role))
                {
                    await next(context);
                    return;
                }

                var claimsIdentity = (ClaimsIdentity)context.User.Identity;

                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            await next(context);
        }
    }
}