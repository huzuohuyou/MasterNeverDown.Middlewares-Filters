# MasterNeverDown.TokenBlackList



## Features

- TokenBlackListMiddleware
  The TokenBlackListMiddleware class is responsible for checking if a token is blacklisted and preventing access if it is. It also provides a method to blacklist tokens for a specified duration. This middleware is useful for implementing logout functionality by blacklisting tokens upon logout.

## Usage
```csharp
dotnet add package MasterNeverDown.TokenBlackList --version 3.0.0
```
- you can add middleware to startup.cs but path must match /account/logout
```csharp
// dependency injection
builder.Services.AddTokenBlackList();
..

app.UseMiddleware<TokenBlackListMiddleware>(); //path must match /account/logout

```
- or you can add LogoutFilter to controller ,this is much more easy🎉
```csharp
/// <summary>
/// Logs out the user by invalidating the provided JWT token.
/// This action uses a custom logout filter.
/// </summary>
/// <param name="authorization">The authorization header containing the JWT token.</param>
/// <returns>Returns Ok if successful, otherwise BadRequest.</returns>
[Authorize]
[TypeFilter(typeof(LogoutFilter))]
[HttpPost("custom/logout")]
[Tags("Filter Demo")]
public IActionResult Logout2([FromHeader]string authorization)
{
    var token = authorization.Replace("Bearer ", "");
    if (string.IsNullOrEmpty(token))
    {
        return BadRequest("Token is required");
    }

    return Ok("Logged out successfully");
}
```

## License
This project is licensed under the MIT License.