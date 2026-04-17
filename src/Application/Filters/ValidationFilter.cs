using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Template.Application.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.Count == 0)
        {
            await next();
            return;
        }

        foreach (object? argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            Type argumentType = argument.GetType();
            Type validatorServiceType = typeof(IValidator<>).MakeGenericType(argumentType);

            object? validatorObject = _serviceProvider.GetService(validatorServiceType);
            if (validatorObject is not IValidator validator)
            {
                continue;
            }

            IValidationContext validationContext = CreateValidationContext(argumentType, argument);

            ValidationResult validationResult =
                await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (validationResult.IsValid)
            {
                continue;
            }

            foreach (ValidationFailure error in validationResult.Errors)
            {
                string key = string.IsNullOrWhiteSpace(error.PropertyName)
                    ? argumentType.Name
                    : error.PropertyName;

                context.ModelState.AddModelError(key, error.ErrorMessage);
            }
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            });

            return;
        }

        await next();
    }

    private static IValidationContext CreateValidationContext(Type modelType, object model)
    {
        Type validationContextType = typeof(ValidationContext<>).MakeGenericType(modelType);

        object? instance = Activator.CreateInstance(validationContextType, model);
        if (instance is not IValidationContext validationContext)
        {
            throw new InvalidOperationException(
                $"Could not create validation context for model type '{modelType.FullName}'.");
        }

        return validationContext;
    }
}