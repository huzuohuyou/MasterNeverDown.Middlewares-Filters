# MasterNeverDown.LoginLimit

## Features

- LoginLimitMiddleware
  The LoginLimitMiddleware class is a middleware component in an ASP.NET Core application that limits login attempts and locks out users after a certain number of failed attempts.
## Getting Started

## Usage
```csharp
dotnet add package MasterNeverDown.LoginLimit --version 3.0.0
```

- you can add middleware to startup.cs but path must match /account/login
```csharp
//return 403 if login failed more than 3 times
app.UseMiddleware<LoginLimitMiddleware>(); 

```

- or add filter to controller ,this is much more easy🎉
```csharp
    [AllowAnonymous]
    [TypeFilter(typeof(LoginLimitFilter))]//return 403 if login failed more than 3 times
    [HttpPost("custom/login")]
    [Tags("Filter Demo")]
    public IActionResult Login2([FromBody] LoginModel login)
    {
        if (IsValidUser(login))
        {
            var token = GenerateJwtToken();
            return Ok(new { token });
        }
    
        return Unauthorized();
        }
```

## License
This project is licensed under the MIT License.