using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace cst350groupapp.Filters
{
    public class SessionCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext Context)
        {
            if (Context.HttpContext.Session.GetString("User") == null)
            {
                Context.Result = new RedirectResult("/User/Login");
            }
        }
    }
}
