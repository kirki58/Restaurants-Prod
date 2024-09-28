using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Restaurants.Application.DTOs;
using Restaurants.Domain.Entitites;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(RestaurantsDbContext dbContext) : IRestaurantsRepository
{
    public async Task<(IEnumerable<Restaurant>, int)> GetAllAsync(int pageSize, int pageNo, string? searchPhrase, int? category, string? sortBy, bool? sortDesc)
    {
        var baseQuery = dbContext.Restaurants
            .Where(r => 
                (searchPhrase == null || r.Name.ToLower().Contains(searchPhrase.ToLower()) ||  r.Description.ToLower().Contains(searchPhrase.ToLower()))
                &&
                (category == null || (int) r.Category == category)
            );

        var count = await baseQuery.CountAsync();

        var sortByColumnPredicates = new Dictionary<string, Expression<Func<Restaurant, object>>>
        {
            {nameof(RestaurantDTO.Name) , r => r.Name},
            {nameof(RestaurantDTO.Tables), r=> r.Tables},
            {nameof(RestaurantDTO.Dishes), r=> r.Dishes.Count}
        };

        if(sortBy != null){
            baseQuery = sortDesc == true ? baseQuery.OrderByDescending(sortByColumnPredicates[sortBy]) : baseQuery.OrderBy(sortByColumnPredicates[sortBy]);
        }

        var result = await baseQuery
                    .Skip((pageNo -1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return (result, count);
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id){
        return await 
        dbContext.Restaurants
        .Include(r => r.Dishes)
        .Include(r => r.RestaurantAdmin)
        .FirstOrDefaultAsync(res => res.Id == id );
    }
    public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant){
        await dbContext.Restaurants.AddAsync(restaurant);
        await dbContext.SaveChangesAsync();
        return restaurant;
    }
    public async Task DeleteRestaurantAsync(Restaurant restaurant){
        dbContext.Restaurants.Remove(restaurant);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant){
        dbContext.Restaurants.Update(restaurant);
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
