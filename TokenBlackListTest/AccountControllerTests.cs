using MasterNeverDown.TokenBlackList;
using MasterNeverDown.TokenBlackList.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TokenBlackListExample.Controllers;

namespace TokenBlackListTest;

public class AccountControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ITokenBlacklistService> _mockTokenBlacklistService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockTokenBlacklistService = new Mock<ITokenBlacklistService>();
        _controller = new AccountController(_mockConfiguration.Object);
    }

    [Fact]
    public void Login_ValidUser_ReturnsToken()
    {
        var loginModel = new LoginModel { Username = "test", Password = "password" };
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("your_secret_key");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("your_issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("your_audience");

        var result = _controller.Login(loginModel) as OkObjectResult;

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Login_InvalidUser_ReturnsUnauthorized()
    {
        var loginModel = new LoginModel { Username = "invalid", Password = "password" };

        var result = _controller.Login(loginModel);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Logout_ValidToken_ReturnsOk()
    {
        var token = "valid_token";
        _mockTokenBlacklistService.Setup(s => s.BlacklistToken(token)).Verifiable();

        var result = _controller.Logout($"Bearer {token}") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Logged out successfully", result.Value);
        _mockTokenBlacklistService.Verify();
    }

    [Fact]
    public void Logout_MissingToken_ReturnsBadRequest()
    {
        var result = _controller.Logout("") as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Token is required", result.Value);
    }
}