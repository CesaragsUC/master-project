using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Interfaces;
using Microsoft.Extensions.Options;
using Product.Infrastructure.Configurations.Azure;
using Serilog;

namespace Infrastructure.Services
{

    public class BobStorageService : IBobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName;
        private readonly string blobConnection;

        public BobStorageService(IOptions<BlobContainers> blobContainers, BlobServiceClient blobServiceClient)
        {
            BlobContainers blobInfo = blobContainers.Value;
            _blobServiceClient = blobServiceClient;
            blobConnection = blobInfo.ConnectionStrings!;
            containerName = blobInfo.ContainerName!;

        }

        public async Task<string> UploadBase64ImageToBlobAsync(string base64Image,
            string fileName,
            string? originalBlobName = null)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!string.IsNullOrEmpty(originalBlobName)) await DeleteIfExistsAsync(originalBlobName);

            string blobName = Guid.NewGuid().ToString() + ".jpg";
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            using (var stream = new MemoryStream(imageBytes))
            {
                var httpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" };
                await blobClient.UploadAsync(stream, httpHeaders);
            }

            return blobClient.Uri.ToString();
        }

        public async Task DeleteIfExistsAsync(string blobUri)
        {
            try
            {
                Uri uri = new Uri(blobUri);
                string blobName = uri.Segments[^1]; // Pega a última parte do caminho (nome do arquivo)

                var container = _blobServiceClient.GetBlobContainerClient(containerName);
                var blob = container.GetBlobClient(blobName);

                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                Log.Information("Blob {BlobName} deletado com sucesso", blobName);
            }
            catch (Exception ex)
            {
                Log.Error("Erro ao tentar deletar o blob: {Message}", ex);
            }
        }

    }
}
