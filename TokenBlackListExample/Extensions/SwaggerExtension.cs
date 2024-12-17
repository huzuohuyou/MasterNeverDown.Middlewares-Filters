using Microsoft.OpenApi.Models;

namespace TokenBlackListExample.Extensions;

public static class SwaggerExtension
{
    public static void AddSwaggerGenService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(c =>
        {
            foreach (var item in XmlCommentsFilePath)
            {
                c.IncludeXmlComments(item, includeControllerXmlComments: true);
            }
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Define the security scheme
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            // Add the security requirement
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
    
    private static List<string> XmlCommentsFilePath
    {
        get
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var appDir = new DirectoryInfo(basePath);
            var files = appDir.GetFiles("*.xml");
            var configXml = files.Select(a => Path.Combine(basePath, a.FullName)).ToList();

            return configXml;
        }
    }
}