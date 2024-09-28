using Restaurants.Domain.Entitites;

namespace Restaurants.Application.DTOs;

public class RestaurantDTO
{
    public int Id {get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public RestaurantCategory Category { get; set; }
    public bool HasDelivery { get; set; }
    public Address? Address { get; set; }
    public List<DishDTO> Dishes { get; set; } = [];
    public int Tables { get; set; }
    public UserDTO RestaurantAdmin { get; set; } = default!;
    public string? RestaurantLogoImageSasUrl { get; set; }
}
