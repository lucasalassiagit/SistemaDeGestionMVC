using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudMVCApp.Filtros
{
    public class FiltroAdmin : ActionFilterAttribute //Clase base ASP.NET que proporciona metodos para interceptar
                                                     // y modificar el flujo de ejecucion de una accion
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var tipo = context.HttpContext.Session.GetString("Tipo");
            var usuario = context.HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario) || tipo != "admin")
            {
                context.Result = new RedirectToActionResult("Home", "Index", null);
            }

            base.OnActionExecuting(context); // Asegura que el comportamiento del filtro sea consistente con el
                                             // sistema de filtros de ASP.NET Core
        }
    }
}
