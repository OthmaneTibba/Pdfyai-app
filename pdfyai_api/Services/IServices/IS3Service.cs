using Amazon.S3.Model;
using pdfyai_api.Models;

namespace pdfyai_api.Services.IServices
{
    public interface IS3Service
    {
        Task<bool> UploadFileAsync(MyS3Object s3Object);

        Task<GetObjectResponse?> DownloadFileAsync(string fileName);


        Task<bool> DeleteFileAsync(string bucketName, string uniqueName);
    }
}