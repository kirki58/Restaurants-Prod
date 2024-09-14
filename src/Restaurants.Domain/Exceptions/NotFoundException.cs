using System;

namespace Restaurants.Domain.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier) : Exception($"{resourceType} with ID: {resourceIdentifier} not found")
{
}
