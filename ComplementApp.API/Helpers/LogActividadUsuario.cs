using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ComplementApp.API.Helpers
{
    public class LogActividadUsuario : IAsyncActionFilter
    {
        private readonly IGeneralInterface _generalInterface;
        public LogActividadUsuario(IGeneralInterface generalInterface)
        {
            this._generalInterface = generalInterface;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUsuarioRepository>();
            var user = await repo.ObtenerUsuarioBase(userId);
            user.FechaUltimoAcceso = _generalInterface.ObtenerFechaHoraActual();
            await unitOfWork.CompleteAsync();
        }
    }
}