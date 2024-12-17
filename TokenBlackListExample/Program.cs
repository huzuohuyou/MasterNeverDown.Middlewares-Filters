using MasterNeverDown.CommonResponse.Middlewares;
using MasterNeverDown.LoginLimit.Middlewares;
using MasterNeverDown.TokenBlackList;
using MasterNeverDown.TokenBlackList.Middlewares;
using TokenBlackListExample.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMemoryCache();
builder.Services.AddLogging();

builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGenService();

// dependency injection
builder.Services.AddTokenBlackList();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<JwtAuthenticationMiddleware>();
//return 403 if login failed more than 3 times
app.UseMiddleware<LoginLimitMiddleware>(); //path must match /account/login
// add token to blacklist if user logout
app.UseMiddleware<TokenBlackListMiddleware>(); //path must match /account/logout
// // add format response middleware
// app.UseMiddleware<CommonResponseMiddleware>();
// // handle exception
// app.UseMiddleware<ShitHappenedMiddleware>(); 

app.UseHttpsRedirection();
//鉴权
app.UseAuthentication();
//授权
app.UseAuthorization();

app.MapControllers();

app.Run();