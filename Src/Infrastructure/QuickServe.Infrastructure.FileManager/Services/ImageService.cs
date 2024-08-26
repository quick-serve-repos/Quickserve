using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using QuickServe.Application.Interfaces.ImageInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.FileManager.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public ImageService(IConfiguration configuration)
        {
           var connectionString = configuration.GetConnectionString("AzureBlobStorage");
           //_blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(image.FileName));

            using (var stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            return blobClient.Uri.ToString();
        }
        public async Task<string> UpdateImageAsync(string oldImageUrl, IFormFile newImage)
        {
            try
            {
                // Parse old image URL to get blob name
                Uri oldUri = new Uri(oldImageUrl);
                string oldBlobName = oldUri.Segments[oldUri.Segments.Length - 1];

                // Get blob container client
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Get blob client for old image
                var blobClient = containerClient.GetBlobClient(oldBlobName);

                // Upload new image
                using (var stream = newImage.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // Return updated image URL
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                throw new Exception($"An error occurred while updating the image: {ex.Message}");
            }
        }

    }
}
