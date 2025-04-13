using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Filters;

namespace AutentificacaoApi.Validations
{
    public class UsuarioDTOValidator  : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(new { errors });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
