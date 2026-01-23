using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared;

namespace Api
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                var message = string.Join("; ", errors);

                context.Result = new BadRequestObjectResult(ApiResponse<string>.Fail(message));
            }
        }
    }
}
