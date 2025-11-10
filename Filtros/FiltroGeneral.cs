using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudMVCApp.Filtros
{
    public class FiltroGeneral : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                // Si no hay usuario en sesión, redirige al login
                context.Result = new RedirectToActionResult("Login", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
