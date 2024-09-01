using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DotnetBase.Infrastructure.Common.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetBase.Infrastructure.Mvc.Filters
{
    public class ValidateModelFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var validationErrors = context.ModelState.Select((entry) =>
                {
                    var key = entry.Key;
                    var err = context.ModelState[key].Errors.Select(e => e.ErrorMessage).ToArray();

                    return new InvalidModel()
                    {
                        PropertyName = key,
                        ErrorMessage = string.Join(";", err)
                    };
                });

                var json = new ResponseModel() { Errors = validationErrors.ToArray(), StatusCode = System.Net.HttpStatusCode.BadRequest };
                context.Result = new BadRequestObjectResult(json);
            }
            else
            {
                await next();
            }
        }

        private class InvalidModel
        {
            public string PropertyName { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
