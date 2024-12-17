using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MasterNeverDown.CommonResponse.Filters;

public class ExceptionHappenedFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHappenedFilter> _logger;

    public ExceptionHappenedFilter(ILogger<ExceptionHappenedFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";
        var response = context.Exception.Message;
        _logger.LogError(context.Exception, $"An unhandled exception has occurred:{response}");
        context.Result = new ObjectResult("An unhandled exception has occurred.")
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}