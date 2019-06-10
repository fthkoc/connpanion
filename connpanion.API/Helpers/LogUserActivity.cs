using System;
using System.Security.Claims;
using System.Threading.Tasks;
using connpanion.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace connpanion.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            
            var userID = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repository = resultContext.HttpContext.RequestServices.GetService<IConnpanionRepository>();
            var user = await repository.GetUser(userID);
            user.LastActive = DateTime.Now;
            await repository.SaveAll();
        }
    }
}