using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BestShop.MyHelpers
{
    public class RequiredNoAuthAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {

        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? role = context.HttpContext.Session.GetString("role");
            if (role != null)
            {
                // the user is already authenticated and we redirected user to home page
                context.Result = new RedirectResult("/");
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {

        }
    }
}
