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
        _logger.LogWarning("Model is not valid: {modelState}", context.ModelState);
        // var model = new ArgumentException(context.ModelState.ToString());
        context.Result = new BadRequestObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
