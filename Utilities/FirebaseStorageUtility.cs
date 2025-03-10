using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace WebPortal.Utilities
{
    public class FirebaseStorageUtility
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "worldgate-43164.firebasestorage.app";

        public FirebaseStorageUtility()
        {
            string credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "firebase-config.json");

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }

            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string objectName = $"channel-icons/{fileName}";

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);
            }

            var encodedObjectName = objectName.Replace("/", "%2F");
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{encodedObjectName}?alt=media";
        }
    }
}
