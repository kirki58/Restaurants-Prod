using MediatR;

namespace Restaurants.Application.Commands;

public class UploadRestaurantLogoCommand : IRequest<string>
{
    public UploadRestaurantLogoCommand(int restaurantId, Stream file)
    {
        RestaurantId = restaurantId;
        this.LogoFile = file;
    }

    public int RestaurantId { get; set; }
    public Stream LogoFile { get; set; }
}
