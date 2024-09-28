using System.Runtime.InteropServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Commands;
using Restaurants.Application.DTOs;
using Restaurants.Application.Queries;
using Restaurants.Infrastructure.Autharization.Constants;

namespace Restaurants.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RestaurantDTO>))]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllRestaurantsQuery query){
            var pagedResult = await mediator.Send(query);
            return Ok(pagedResult);
        }
        [Authorize]
        [HttpGet("{id}", Name = "GetRestaurant")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestaurantDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRestaurantByIdAsync([FromRoute]int id){
            var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
            return Ok(restaurant);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RestaurantDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize(Policy = AppPolicies.HasNationality)]
        [Authorize(Policy = AppPolicies.OlderThanEighteen)]
        public async Task<IActionResult> CreateRestaurantAsync([FromBody] CreateRestaurantCommand command){
            var newItem = await mediator.Send(command);
            return CreatedAtRoute("GetRestaurant", new{id = newItem.Id}, newItem);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize] // Authorizes Admin or restaurant owner internally
        public async Task<IActionResult> DeleteRestaurantAsync([FromRoute] int id){
            await mediator.Send(new DeleteRestaurantCommand(id));

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize] // Authorizes Admin or restaurant owner internally
        public async Task<IActionResult> UpdateRestaurantAsync([FromRoute] int id, [FromBody] UpdateRestaurantDTO dto){
            UpdateRestaurantCommand command = new UpdateRestaurantCommand(id, dto);
            await mediator.Send(command);

            return NoContent();
        }

        [HttpPost("{id}/logos")]
        [Authorize] // Authorizes Admin or restaurant owner internally
        public async Task<IActionResult> UploadRestaurantLogo([FromRoute] int id, IFormFile formFile){
            using var fileStream = formFile.OpenReadStream();

            var command = new UploadRestaurantLogoCommand(id, fileStream);
            var url = await mediator.Send(command);

            return Created(url, null);
        }
    }
}
