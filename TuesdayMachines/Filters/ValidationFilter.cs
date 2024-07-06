
using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Filters
{
    public static class DataValidator
    {
        public static (List<ValidationResult> Results, bool IsValid) DataAnnotationsValidate(this object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            var isValid = Validator.TryValidateObject(model, context, results, true);

            return (results, isValid);
        }
    }

    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
        {
            var argument = invocationContext.Arguments.OfType<T>().FirstOrDefault();
            var response = argument.DataAnnotationsValidate();

            if (!response.IsValid)
            {
                string errorMessage = response.Results.FirstOrDefault().ErrorMessage;
                return Results.Json(new { error = "invalid_model", message = errorMessage });
            }

            return await next(invocationContext);
        }
    }
}
