using System.Text.Json;
using MasterNeverDown.CommonResponse.Middlewares;
using MasterNeverDown.TokenBlackList;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace TokenBlackListTest;

public class CommonResponseMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsSuccessfulResponse_WhenStatusCodeIs200()
    {
        var context = new DefaultHttpContext();
        context.Response.StatusCode = StatusCodes.Status200OK;
        var middleware =
            new CommonResponseMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<CommonResponseMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal(StatusCodes.Status200OK, response.GetProperty("code").GetInt32());
        Assert.Equal("successful", response.GetProperty("msg").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ReturnsErrorResponse_WhenStatusCodeIsNot200()
    {
        var context = new DefaultHttpContext();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var middleware =
            new CommonResponseMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<CommonResponseMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal(StatusCodes.Status500InternalServerError, response.GetProperty("code").GetInt32());
        Assert.Equal("error", response.GetProperty("msg").GetString());
    }

    [Fact]
    public void IsJson_ReturnsTrue_ForValidJson()
    {
        var json = "{\"key\":\"value\"}";

        var result = CommonResponseMiddleware.IsJson(json);

        Assert.True(result);
    }

    [Fact]
    public void IsJson_ReturnsFalse_ForInvalidJson()
    {
        var json = "invalid json";

        var result = CommonResponseMiddleware.IsJson(json);

        Assert.False(result);
    }

    [Fact]
    public void IsJson_ReturnsFalse_ForEmptyString()
    {
        var json = "";

        var result = CommonResponseMiddleware.IsJson(json);

        Assert.False(result);
    }
}