
using Firebase.Auth;
using Firebase.Storage;

namespace eSMP.Services.FileRepo
{
    public class FileRepository : IFileReposity
    {
        private static string ApiKey = "AIzaSyBaUaNJe050MkvaSfL2LOw24AnXKN2Sl60";
        private static string Bucket = "esmp-4b85e.appspot.com";
        private static string AuthEmail = "imageadmin@gmail.com";
        private static string AuthPassword = "123456";

        public async Task<string> UploadFile(IFormFile formFile, string fileName)
        {
            var file = formFile.OpenReadStream();
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            FirebaseAuthLink a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("images")
                .Child(fileName)
                .PutAsync(file, cancellation.Token);
            try
            {

                string link = await task;
                return link;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> DeleteFileASYNC(string fileName)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                FirebaseAuthLink a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child(fileName)
                    .DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
