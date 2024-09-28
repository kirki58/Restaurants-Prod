using System;
using Microsoft.OpenApi.Models;
using Restaurants.API.Middlewares;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Extensions;
using Serilog;

namespace Restaurants.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddBuilderExtensions(this WebApplicationBuilder builder){
        // Integrate custom settings in appsettings.json to Middlewares/MiddlewareSettings

        builder.Services.Configure<MiddlewareSettings>(builder.Configuration.GetSection("MiddlewareSettings"));

        // Add services to the container.

        builder.Services.AddScoped<ErrorHandlerMw>(); // Register error handling middleware as a dependancy

        builder.Services.AddScoped<RequestTimeoutMw>(); // Register request timeout catcher middleware as a dependancy

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(conf => {
            conf.AddSecurityDefinition("httpBearer", new OpenApiSecurityScheme{
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            conf.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "httpBearer"
                        }
                    },
                    []
                }
            });
        });

        builder.Services.AddApplication();      // Aplication layer ServiceCollection Extension

        builder.Services.AddInfraStructure(builder.Configuration);   // Infrastructure layer ServiceCollection Extension

        builder.Services.AddEndpointsApiExplorer(); // Explore and integrate API Endpoints metadata with swagger. (Including minimal API endpoints)

        // Serilog configuration done in builder.Host
        builder.Host.UseSerilog((context, configuration) =>  
            configuration
                .ReadFrom.Configuration(context.Configuration)
        );
    }
}
