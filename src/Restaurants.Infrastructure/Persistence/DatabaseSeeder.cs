using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;


namespace Restaurants.Infrastructure.Persistence;

internal class DatabaseSeeder : IDatabaseSeeder
{
    public async Task Seed(RestaurantsDbContext context)
    {
        // Ensure that the DB is up to date before seeding data
        // Pending migrations are directly applied (if any)
        if(context.Database.GetPendingMigrations().Any()){
            await context.Database.MigrateAsync();
        }
        
        if(!context.Roles.Any()){
            List<IdentityRole> roles = [
                new IdentityRole{
                    Id = Guid.NewGuid().ToString(),
                    Name = AppRoles.User,
                    NormalizedName = AppRoles.User.ToUpper(),
                },
                
                new IdentityRole{
                    Id = Guid.NewGuid().ToString(),
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper(),
                }
            ];
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}
