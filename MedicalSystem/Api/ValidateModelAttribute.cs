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
                string error = context.ModelState
                    .SelectMany(kvp => kvp.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Неверные данные";

                context.Result = new BadRequestObjectResult(new ApiResponse<string>(
                    Success: false,
                    Data: null,
                    ErrorMessage: error
                ));
            }
        }
    }
}
