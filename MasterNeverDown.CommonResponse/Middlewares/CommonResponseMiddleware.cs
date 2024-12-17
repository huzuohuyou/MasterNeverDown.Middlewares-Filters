using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MasterNeverDown.CommonResponse.Middlewares;

public class CommonResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CommonResponseMiddleware> _logger;

    public CommonResponseMiddleware(RequestDelegate next, ILogger<CommonResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        await _next(context);
        context.Response.Body = originalBodyStream;
        var response = new
        {
            code = context.Response.StatusCode,
            msg = context.Response.StatusCode == StatusCodes.Status200OK ? "successful" : "error",
            data = await FormatResponse(responseBody)
        };
        var jsonResponse = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonResponse);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static async Task<object?> FormatResponse(Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        if (!string.IsNullOrWhiteSpace(text)&&IsJson(text))
        {
            return JsonSerializer.Deserialize<object>(text);
        }

        return text;
    }
    public static bool IsJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        try
        {
            JsonDocument.Parse(text);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
    
}