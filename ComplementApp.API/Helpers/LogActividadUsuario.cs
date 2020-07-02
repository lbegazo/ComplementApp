using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ComplementApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ComplementApp.API.Helpers
{
    public class LogActividadUsuario : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUsuarioRepository>();
            var user = await repo.ObtenerUsuario(userId);
            user.FechaUltimoAcceso = DateTime.Now;
            await unitOfWork.CompleteAsync();

        }
    }
}