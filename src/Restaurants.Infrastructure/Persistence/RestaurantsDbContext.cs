using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Entitites;

namespace Restaurants.Infrastructure.Persistence;


// IdentityDbContext inherits DbContext, it has built-in auth logic
internal class RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options) : IdentityDbContext<User>(options)
{
    internal DbSet<Restaurant> Restaurants { get; set; }
    internal DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>()
            .OwnsOne(r => r.Address);
        
        modelBuilder.Entity<Restaurant>()
            .HasMany(r => r.Dishes)
            .WithOne()
            .HasForeignKey(d => d.RestaurantId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnedRestaurants)
            .WithOne(r => r.RestaurantAdmin)
            .HasForeignKey(r => r.AdminId);
    }

    // internal async static Task SeedData(RestaurantsDbContext context){
    //     // Ensure that the DB is up to date before seeding data
    //     // Pending migrations are directly applied (if any)
    //     if(context.Database.GetPendingMigrations().Any()){
    //         await context.Database.MigrateAsync();
    //     }
        
    //     if(!context.Roles.Any()){
    //         List<IdentityRole> roles = [
    //             new IdentityRole{
    //                 Id = Guid.NewGuid().ToString(),
    //                 Name = AppRoles.User,
    //                 NormalizedName = AppRoles.User.ToUpper(),
    //             },
                
    //             new IdentityRole{
    //                 Id = Guid.NewGuid().ToString(),
    //                 Name = AppRoles.Admin,
    //                 NormalizedName = AppRoles.Admin.ToUpper(),
    //             }
    //         ];
    //         await context.Roles.AddRangeAsync(roles);
    //         await context.SaveChangesAsync();
    //     }
    // }
}
