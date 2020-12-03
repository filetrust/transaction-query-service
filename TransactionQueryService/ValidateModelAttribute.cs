using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Glasswall.Administration.K8.TransactionQueryService
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.ModelState.IsValid) return base.OnActionExecutionAsync(context, next);
            
            var errors = context.ModelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList();

            context.Result = new BadRequestObjectResult(errors);

            return base.OnActionExecutionAsync(context, next);
        }
    }
}