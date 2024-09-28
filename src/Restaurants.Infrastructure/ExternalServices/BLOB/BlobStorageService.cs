using System;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Abstractions;
using Restaurants.Infrastructure.Settings;

namespace Restaurants.Infrastructure.ExternalServices.BLOB;

public class BlobStorageService(
    IOptions<BlobStorageSettings> options,
    IMemoryCache memoryCache) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = options.Value;
    private readonly int _sasAndCacheLifeTime = 30; // Minutes
    public async Task<string> UploadToBlobStorageAsync(Stream data)
    {
        var blobClient = new BlobServiceClient(_settings.ConnectionString)
                        .GetBlobContainerClient(_settings.LogoImagesContainerName)
                        .GetBlobClient(Guid.NewGuid().ToString());
            
        await blobClient.UploadAsync(data);

        return blobClient.Uri.ToString();
    }
    public string? GetBlobSasUrl(string? blobUrl){
        if(blobUrl == null) return null;

        string blobName;

        try{
            blobName = GetBlobName(blobUrl);
        }
        catch{
            return null;
        }
        // Check if the SAS URL is present in the cache and return that if so
        if(memoryCache.TryGetValue(blobName, out string? sasUrl)){
            return sasUrl; // Return URL stored in cache
        }

        var sasBuilder = new BlobSasBuilder(){
            BlobContainerName = _settings.LogoImagesContainerName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_sasAndCacheLifeTime),
            BlobName = blobName
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var newSasToken = sasBuilder.ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(blobServiceClient.AccountName, _settings.AccountKey)).ToString();
        var newSasUrl = $"{blobUrl}?{newSasToken}";
        memoryCache.Set(blobName, newSasUrl, TimeSpan.FromMinutes(_sasAndCacheLifeTime));

        return newSasUrl;
    }

    private static string GetBlobName(string blobUrl){
        var uri = new Uri(blobUrl);
        return uri.Segments.Last();
    }
}
