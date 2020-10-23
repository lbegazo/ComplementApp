using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ComplementApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ComplementApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private readonly IGeneralInterface _generalInterface;
        public LogUserActivity(IGeneralInterface generalInterface)
        {
            this._generalInterface = generalInterface;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = _generalInterface.ObtenerFechaHoraActual();
            await repo.SaveAll();

        }
    }
}