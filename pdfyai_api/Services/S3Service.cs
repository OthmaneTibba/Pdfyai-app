using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using pdfyai_api.Models;
using pdfyai_api.Services.IServices;

namespace pdfyai_api.Services
{
    public class S3Service : IS3Service
    {

        private readonly IConfiguration _configuartion;

        private readonly AmazonS3Client _s3Client;


        public S3Service(IConfiguration configuration, AmazonS3Client s3Client)
        {
            _configuartion = configuration;
            _s3Client = s3Client;
        }

        public async Task<bool> DeleteFileAsync(string bucketName, string uniqueName)
        {
            var res = await _s3Client.DeleteObjectAsync(bucketName, uniqueName);

            if (res.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }

        public async Task<GetObjectResponse?> DownloadFileAsync(string fileName)
        {


            var response = await new TransferUtility(_s3Client).S3Client.GetObjectAsync(
                new GetObjectRequest()
                {
                    BucketName = "pdfyaifiles",
                    Key = fileName,
                }
            );

            if (response.ResponseStream == null)
                return null;


            return response;

        }



        public async Task<bool> UploadFileAsync(MyS3Object s3Object)
        {
            try
            {


                var upload = new TransferUtilityUploadRequest()
                {
                    InputStream = s3Object.InputStream,
                    BucketName = s3Object.BucketName,
                    // HADA KHSO IKON UNIQUE
                    Key = s3Object.Name,
                    CannedACL = S3CannedACL.NoACL
                };



                var transferUtility = new TransferUtility(_s3Client);

                await transferUtility.UploadAsync(upload);

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}