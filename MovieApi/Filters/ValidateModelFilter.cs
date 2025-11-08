// This file is part of the project. Copyright (c) Company.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieApi.Filters;

public class ValidateModelFilter : IActionFilter
{
    private readonly ILogger<ValidateModelFilter> _logger;

    public ValidateModelFilter(ILogger<ValidateModelFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        var firstError = context.ModelState
            .Where((kvp) => kvp.Value?.Errors?.Count > 0)
            .Select(kvp => kvp.Value!.Errors.FirstOrDefault()?.ErrorMessage)
            .FirstOrDefault();


        _logger.LogWarning("Model is not valid: {modelState}", firstError);
        // var model = new ArgumentException(context.ModelState.ToString());
        context.Result = new BadRequestObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
