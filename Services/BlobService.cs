using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobService
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobService(IConfiguration configuration)
    {
        
        string scheme = "https";
        string accountName = "stepproj";
        string endpointSuffix = "core.windows.net";
        string sasToken = "sp=racwdl&st=2025-01-25T14:54:06Z&se=2025-01-25T22:54:06Z&sip=192.168.68.106&sv=2022-11-02&sr=c&sig=oWDzs9QaVGyMVwo8gXVnNjrfEYp5FrcyhcTRlPQmYz8%3D";
        var blobEndpoint = ConstructBlobEndpoint(scheme, accountName, endpointSuffix, sasToken);

        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];
        _blobContainerClient = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, true);
        return blobClient.Uri.ToString();
    }
    internal static (Uri Primary, Uri Secondary) ConstructBlobEndpoint(string scheme, string accountName, string endpointSuffix, string sasToken)
    {
        if (string.IsNullOrEmpty(scheme))
        {
            throw new ArgumentNullException(nameof(scheme));
        }

        if (string.IsNullOrEmpty(accountName))
        {
            throw new ArgumentNullException(nameof(accountName));
        }

        if (string.IsNullOrEmpty(endpointSuffix))
        {
            endpointSuffix = "core.windows.net";
        }

        return ConstructUris(scheme, accountName, "blob", endpointSuffix, sasToken);
    }

    private static (Uri Primary, Uri Secondary) ConstructUris(string scheme, string accountName, string hostNamePrefix, string endpointSuffix, string sasToken)
    {
        var primaryUriBuilder = new UriBuilder
        {
            Scheme = scheme,
            Host = $"{accountName}.{hostNamePrefix}.{endpointSuffix}",
            Query = sasToken
        };

        var secondaryUriBuilder = new UriBuilder
        {
            Scheme = scheme,
            Host = $"{accountName}-secondary.{hostNamePrefix}.{endpointSuffix}",
            Query = sasToken
        };

        return (primaryUriBuilder.Uri, secondaryUriBuilder.Uri);
    }
}