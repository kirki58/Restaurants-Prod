using System;
using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Entitites;

public class Restaurant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public RestaurantCategory Category { get; set; }
    public bool HasDelivery { get; set; }

    public string? ContactNumber { get; set; }
    public string? ContactEmail { get; set; }
    public Address? Address { get; set; }
    public List<Dish> Dishes { get; set; } = new();

    public int Tables { get; set; }

    // A Restaurant can have only 1 admin
    // A User can be the admin of many restaurants
    // As such One-to-Many relationship

    // Navigation property for the Restaurant Admin
    public User RestaurantAdmin {get; set;} = default!;
    // Actual Foreign Key to Restaurant Admin
    public string AdminId {get; set;} = default!;
    // Url to restaurant logo in Azure BLOB Storage
    public string? LogoUrl { get; set; }
}

public enum RestaurantCategory{
    Italian,
    Turkish,
    FastFood,
    Chinese
}
