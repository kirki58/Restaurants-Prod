using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Restaurants.Infrastructure.Persistence;
internal interface IDatabaseSeeder
{
    Task Seed(RestaurantsDbContext context);
}
