using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Restaurants.Domain.Abstractions;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Autharization;
using Restaurants.Infrastructure.Autharization.Constants;
using Restaurants.Infrastructure.Autharization.Requirements;
using Restaurants.Infrastructure.ExternalServices.BLOB;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Repositories;
using Restaurants.Infrastructure.Settings;

namespace Restaurants.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfraStructure(this IServiceCollection services, IConfiguration configuration){
        var connectionString = configuration.GetConnectionString("RestaurantsDb");
        services.AddDbContext<RestaurantsDbContext>( options => 
            options
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
        );

        // Add AspNetCore.Identity.EntityFrameworkCore API User Endpoints bound to repositories of RestaurantsDbContext
        services.AddIdentityApiEndpoints<User>()
        .AddRoles<IdentityRole>()
        .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>()
        .AddEntityFrameworkStores<RestaurantsDbContext>();

        services.AddAuthorizationBuilder()
        .AddPolicy(AppPolicies.HasNationality, policy => policy.RequireClaim(AppClaimTypes.Nationality))
        .AddPolicy(AppPolicies.IsTurkish, policy => policy.RequireClaim(AppClaimTypes.Nationality, "Turkish"))
        .AddPolicy(AppPolicies.OlderThanEighteen, policy => policy.RequireAssertion(context => { 
            var birthDate = context.User.Claims.FirstOrDefault(c => c.Type == AppClaimTypes.BirthDate);

            if(birthDate == null){
                return false;
            }
            
            var parsedDate = DateOnly.Parse(birthDate.Value);
            var now = DateOnly.FromDateTime(DateTime.Now);

            var passedYears = now.Year - parsedDate.Year;
             
            if(now < parsedDate.AddYears(passedYears)){
                passedYears--;
            }

            return passedYears >= 18;
        }))
        .AddPolicy(AppPolicies.OwnsTwoOrMoreRestaurants, policy => policy.AddRequirements(new RestaurantOwnerAuthorization(2)));
        
        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        services.AddScoped<IDishesRepository, DishesRepository>();

        services.Configure<BlobStorageSettings>(configuration.GetSection("BlobStorage"));
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddMemoryCache();

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
    }
}
