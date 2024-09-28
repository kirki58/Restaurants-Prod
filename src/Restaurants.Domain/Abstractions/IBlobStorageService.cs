namespace Restaurants.Domain.Abstractions;

public interface IBlobStorageService
{
    Task<string> UploadToBlobStorageAsync(Stream data);
    public string? GetBlobSasUrl(string? blobUrl);
}
