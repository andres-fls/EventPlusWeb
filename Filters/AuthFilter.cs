using System.Web.Mvc;
using System.Web.Routing;

namespace EventPlusWeb1.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            if (session["UsuarioId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Usuarios" },
                        { "action", "Login" }
                    });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}