namespace Restaurants.Infrastructure.Settings;

public class BlobStorageSettings
{
    public string ConnectionString { get; set; } = default!;
    public string LogoImagesContainerName { get; set; } = default!;
    public string AccountKey { get; set; } = default!;
}
