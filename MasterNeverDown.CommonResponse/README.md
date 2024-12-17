# MasterNeverDown.CommonResponse

## Features

- CommonResponseMiddleware
  CommonResponseMiddleware is a middleware component in an ASP.NET Core application that formats the HTTP response. It ensures that the response is in a consistent format, typically JSON, and includes a status code and message indicating the result of the request.
- ShitHappenedMiddleware
  ShitHappenedMiddleware is a middleware component in an ASP.NET Core application that handles exceptions and ensures that the response is consistently structured, typically including a status code and a message indicating an error.
## Getting Started

## Usage
```csharp
dotnet add package MasterNeverDown.CommonResponse --version 3.0.0
```
``` cs
// add format response middleware
app.UseMiddleware<CommonResponseMiddleware>();
// handle exception
app.UseMiddleware<ShitHappenedMiddleware>(); 
```
- common response format
```json
{
  "code": 200,
  "msg": "successful",
  "data": "your data"
}
```

## License
This project is licensed under the MIT License.