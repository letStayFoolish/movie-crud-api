using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieApi.Filters;

public class LoggingFilter : IActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Executing {ActionDescriptorDisplayName}", context.ActionDescriptor.DisplayName);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Executed {ActionDescriptorDisplayName}", context.ActionDescriptor.DisplayName);
    }
}
