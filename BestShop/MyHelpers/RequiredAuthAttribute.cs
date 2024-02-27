using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BestShop.MyHelpers
{
    public class RequiredAuthAttribute : Attribute, IPageFilter
    {
        public string RequiredRole { get; set; } = "";
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? role = context.HttpContext.Session.GetString("role");

            if (role == null)
            {
                // user not authonticated => redirect user to home page
                context.Result = new RedirectResult("/");
            }
            else
            {
                if(RequiredRole.Length > 0 && !RequiredRole.Equals(role))
                {
                    // The user is authonticated but the role is not autorized
                    context.Result = new RedirectResult("/");
                }
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            
        }
    }
}
