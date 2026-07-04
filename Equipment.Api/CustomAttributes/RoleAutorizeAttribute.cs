using Equipment.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Equipment.Api.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAutorizeAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int[] _rolesId;
        public RoleAutorizeAttribute(int[] roleId) 
        {
            _rolesId = roleId;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            string? token = context.HttpContext.Request.Headers["Autorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new { error = "Сессия не передана" }) { StatusCode = 401 };
                return;
            }

            var session = await dbContext.Sessions.Include(s => s.User).FirstOrDefaultAsync(s => s.Token == token);

            if (session == null)
            {
                context.Result = new JsonResult(new { error = "Сессия не найдена" }) { StatusCode = 401 };
                return;
            }

            if (!_rolesId.Contains(session.User.Role_id))
            {
                context.Result = new JsonResult(new { error = "Недостаточно прав" }) { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}
