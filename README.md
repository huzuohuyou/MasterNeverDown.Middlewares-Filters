# MasterNeverDown.TokenBlackList

TokenBlackList for logout

## Overview

This project demonstrates how to implement JWT authentication with token blacklisting for logout functionality in an ASP.NET Core application.

## Features

- JWT authentication
- Token blacklisting for logout
- Swagger integration for API documentation

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio or JetBrains Rider

### Installation

1. Clone the repository:
   ```shell
   git clone https://github.com/your-repo/MasterNeverDown.TokenBlackList.git


## Build
1. Navigate to the project directory:  
```csharp
cd MasterNeverDown.TokenBlackList
```

2. Restore the dependencies: 
```csharp
dotnet restore
```


## Running the Application
Build and run the application:  
```
dotnet run
```

Open your browser and navigate to https://localhost:5001/swagger to access the Swagger UI.  
## Usage
- Login
Send a POST request to /Account/login with the following JSON body:


``` cs
//return 403 if login failed more than 3 times
app.UseMiddleware<LoginLimitMiddleware>();
// add token to blacklist if user logout
app.UseMiddleware<TokenBlackListMiddleware>();

```


## License
This project is licensed under the MIT License.