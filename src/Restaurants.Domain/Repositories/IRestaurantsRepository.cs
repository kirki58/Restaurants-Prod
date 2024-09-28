using Restaurants.Domain.Entitites;

namespace Restaurants.Domain.Repositories;

public interface IRestaurantsRepository
{
    Task<(IEnumerable<Restaurant>, int)> GetAllAsync(int pageSize, int pageNo, string? searchPhrase, int? category, string? sortBy, bool? sortDesc);
    Task<Restaurant?> GetRestaurantByIdAsync(int id);
    Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
    Task DeleteRestaurantAsync(Restaurant restaurant);
    Task UpdateRestaurantAsync(Restaurant restaurant);
    Task SaveChangesAsync();
}
