using System;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Commands;
using Restaurants.Application.Services;
using Restaurants.Application.Users;
using Restaurants.Domain.Abstractions;
using Restaurants.Domain.Entitites;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Handlers.CommandHandlers;

public class UploadRestaurantLogoCommandHandler(
    IRestaurantsRepository repository,
    IUserContext userContext,
    ILogger<UploadRestaurantLogoCommandHandler> logger,
    IBlobStorageService blobStorageService
    ) : IRequestHandler<UploadRestaurantLogoCommand, string>
{
    public async Task<string> Handle(UploadRestaurantLogoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading Logo for restaurant with Id: {id}", request.RestaurantId);
        var restaurant = await repository.GetRestaurantByIdAsync(request.RestaurantId);

        if(restaurant == null){
            logger.LogWarning($"Restaurant with ID {request.RestaurantId} not found");
            throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        var user = userContext.GetCurrentUser();
        if(!user.IsAuthorized(restaurant)){
            throw new ForbidException(user.Id, this.GetType().Name, nameof(Restaurant), request.RestaurantId.ToString());
        }

        var url = await blobStorageService.UploadToBlobStorageAsync(request.LogoFile);
        restaurant.LogoUrl = url;
        await repository.SaveChangesAsync();

        return url;
    }
}
