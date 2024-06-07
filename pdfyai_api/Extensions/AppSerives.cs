using Amazon.Runtime;
using Amazon.S3;
using pdfyai.Utils;
using pdfyai_api.Services;
using pdfyai_api.Services.IServices;
using pdfyai_api.Utils;
using Pinecone;

namespace pdfyai_api.Extensions
{
    public static class AppSerives
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {

            var accesKey = configuration["Amazon:AccessKey"]!;
            var secretKey = configuration["Amazon:SecretKey"];
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest3
            };
            var credentials = new BasicAWSCredentials(accesKey, secretKey);
            services.AddScoped(_ => new PineconeClient(configuration["ApiKeys:Pinecone"]!, "gcp-starter"));
            services.AddScoped(_ => new OpenAI_API.OpenAIAPI(configuration["ApiKeys:OpenAi"]!));
            services.AddScoped<AmazonS3Client>(_ => new AmazonS3Client(credentials, config));
            services.AddScoped<IS3Service, S3Service>();
            services.AddSingleton<AppUtils>();
            services.AddScoped<TokenManager>();
            services.AddScoped<AuthUtils>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<PdfAuthils>();

            return services;
        }
    }
}