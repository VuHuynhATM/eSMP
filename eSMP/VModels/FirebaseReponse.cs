namespace eSMP.VModels
{
    public class FirebaseReponse
    {
        public string shortLink { get; set; }
        public string previewLink { get; set; }
    }
    public class FirebaseRequest
    {
        public string longDynamicLink { get; set; }
        public Suffix suffix { get; set; }
    }
    public class Suffix
    {
        public string option { get; set; }
    }
}
