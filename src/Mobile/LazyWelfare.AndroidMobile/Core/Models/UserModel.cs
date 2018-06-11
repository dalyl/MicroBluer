using Android.OS;

namespace LazyWelfare.AndroidMobile.Models
{
    public class UserModel
    {

        public string Name { get; set; }

        public string Host { get; set; }

        public string Icon { get; set; }

        public string Signature { get; set; }

        public string Folder { get; set; }

        public string Root {
            get {
                return $"{Environment.ExternalStorageDirectory.Path}/{Folder}";
            }
        }
    }
}