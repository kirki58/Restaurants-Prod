using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Restaurants.Application.Commands;
using Restaurants.Domain.Entitites;
using Restaurants.Domain.Repositories;
using Xunit.Abstractions;

namespace Restaurants.API.Test.Controllers;

// This class provides Integration testing rather than Unit testing!
public class RestaurantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IRestaurantsRepository> _repositoryMock = new();
    public RestaurantsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureTestServices(services => {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantsRepository), _ => _repositoryMock.Object));
            });
        });
    }

    [Fact]
    public async Task GetAllAsync_ForValidRequest_Returns200Ok(){
        // Arrange
        var restaurants = new List<Restaurant>{
            new Restaurant{
                Id = 1,
                Name = "name",
                Description = "desc",
                Category = RestaurantCategory.Italian
            },

            new Restaurant{
                Id = 2,
                Name = "name",
                Description = "desc",
                Category = RestaurantCategory.Italian
            },

            new Restaurant{
                Id = 3,
                Name = "name",
                Description = "desc",
                Category = RestaurantCategory.Turkish
            },

            new Restaurant{
                Id = 4,
                Name = "name",
                Description = "desc",
                Category = RestaurantCategory.Turkish
            },

            new Restaurant{
                Id = 5,
                Name = "name",
                Description = "desc",
                Category = RestaurantCategory.Chinese
            },
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, null, null)).ReturnsAsync((restaurants, restaurants.Count));
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/restaurants?pageNo=1&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllAsync_ForInvalidRequest_Returns500Internal(){
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/restaurants");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByIdAsync_ForValidRequest_Returns200Ok(){
        //
        var client = _factory.CreateClient();
        var id = 1;

        var restaurant = new Restaurant{
            Id = 1,
            Name = "name",
            Description = "desc" 
        };

        _repositoryMock.Setup(r => r.GetRestaurantByIdAsync(id)).ReturnsAsync(restaurant);

        //
        var response = await client.GetAsync($"/api/restaurants/{id}");

        //
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByIdAsync_ResourceNotInDb_Returns404NotFound(){
        //
        var client = _factory.CreateClient();
        var id = 1;

        _repositoryMock.Setup(r => r.GetRestaurantByIdAsync(id)).ReturnsAsync((Restaurant?) null);

        //
        var response = await client.GetAsync($"/api/restaurants/{id}");

        //
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
