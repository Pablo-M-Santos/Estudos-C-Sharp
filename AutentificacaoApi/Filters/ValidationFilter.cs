using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutentificacaoApi.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var erros = context.ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                context.Result = new BadRequestObjectResult(new { erros });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
