using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Extensions;

public static class AppCollectionExtensions
{
    public static async Task UseInfrastructureAsync(this WebApplication app){
        // Map AspNetCore.Identity.EntityFrameworkCore API User Endpoints
        app.MapGroup("api/auth").MapIdentityApi<User>();

        // SEED DATA
        await using (var scope = app.Services.CreateAsyncScope()){
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<RestaurantsDbContext>();
            var seeder = serviceProvider.GetRequiredService<IDatabaseSeeder>();
            await seeder.Seed(context);
        }
    }
}
