namespace Catalog.Domain.Abstractions;

public interface IBobStorageService
{
    Task DeleteIfExistsAsync(string blobName);
    Task<string> UploadBase64ImageToBlobAsync(string base64Image, string fileName, string? originalBlobName = null);
}
