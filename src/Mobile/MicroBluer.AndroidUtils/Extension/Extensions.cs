namespace MicroBluer.AndroidUtils.Extension
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Net;
    using Android.Widget;
    using Java.IO;
    public static partial  class Extensions
    {
        public static void OpenFileByPath(this Context context, string path)
        {
            if (context == null || path == null)
                return;
            Intent intent = new Intent();
            intent.AddFlags(ActivityFlags.NewTask);
            //设置intent的Action属性
            intent.SetAction(Intent.ActionView);
            //文件的类型
            string type = GetType(path);

            try
            {
                //设置intent的data和Type属性
                intent.SetDataAndType(Uri.FromFile(new File(path)), type);
                //跳转
                context.StartActivity(intent);
            }
            catch (System.Exception e)
            {
                Toast.MakeText(context, "无法打开该格式文件", ToastLength.Short).Show();
            }
        }

        static string GetType(string path)
        {
            foreach (var item in MATCH_ARRAY)
            {
                if (path.EndsWith(item.Key))
                {
                    return item.Value;
                }
            }
            return string.Empty;
        }

        static void OpenFile(this Activity the, string fileName)
        {

            File file = new File(fileName);
            if (file.Exists())
            {
                Uri path = Uri.FromFile(file);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(path, "text/plain");
                intent.SetFlags(ActivityFlags.ClearTop);
                the.StartActivity(intent);
            }
            else
            {
                Toast.MakeText(the, "文件不存在或已被删除", ToastLength.Short).Show();
            }

        }

        public static Dictionary<string, string> dic = new Dictionary<string, string>();

        public static List<(string type, string tail)> maps = new List<(string type, string tail)> {
("application/envoy","evy"),
("application/fractals","fif"),
("application/futuresplash","spl"),
("application/hta","hta"),
("application/internet-property-stream","acx"),
("application/mac-binhex40","hqx"),
("application/msword","doc"),
("application/msword","dot"),
("application/octet-stream","*"),
("application/octet-stream","bin"),
("application/octet-stream","class"),
("application/octet-stream","dms"),
("application/octet-stream","exe"),
("application/octet-stream","lha"),
("application/octet-stream","lzh"),
("application/oda","oda"),
("application/olescript","axs"),
("application/pdf","pdf"),
("application/pics-rules","prf"),
("application/pkcs10","p10"),
("application/pkix-crl","crl"),
("application/postscript","ai"),
("application/postscript","eps"),
("application/postscript","ps"),
("application/rtf","rtf"),
("application/set-payment-initiation","setpay"),
("application/set-registration-initiation","setreg"),
("application/vnd.ms-excel","xla"),
("application/vnd.ms-excel","xlc"),
("application/vnd.ms-excel","xlm"),
("application/vnd.ms-excel","xls"),
("application/vnd.ms-excel","xlt"),
("application/vnd.ms-excel","xlw"),
("application/vnd.ms-outlook","msg"),
("application/vnd.ms-pkicertstore","sst"),
("application/vnd.ms-pkiseccat","cat"),
("application/vnd.ms-pkistl","stl"),
("application/vnd.ms-powerpoint","pot"),
("application/vnd.ms-powerpoint","pps"),
("application/vnd.ms-powerpoint","ppt"),
("application/vnd.ms-project","mpp"),
("application/vnd.ms-works","wcm"),
("application/vnd.ms-works","wdb"),
("application/vnd.ms-works","wks"),
("application/vnd.ms-works","wps"),
("application/winhlp","hlp"),
("application/x-bcpio","bcpio"),
("application/x-cdf","cdf"),
("application/x-compress","z"),
("application/x-compressed","tgz"),
("application/x-cpio","cpio"),
("application/x-csh","csh"),
("application/x-director","dcr"),
("application/x-director","dir"),
("application/x-director","dxr"),
("application/x-dvi","dvi"),
("application/x-gtar","gtar"),
("application/x-gzip","gz"),
("application/x-hdf","hdf"),
("application/x-internet-signup","ins"),
("application/x-internet-signup","isp"),
("application/x-iphone","iii"),
("application/x-javascript","js"),
("application/x-latex","latex"),
("application/x-msaccess","mdb"),
("application/x-mscardfile","crd"),
("application/x-msclip","clp"),
("application/x-msdownload","dll"),
("application/x-msmediaview","m13"),
("application/x-msmediaview","m14"),
("application/x-msmediaview","mvb"),
("application/x-msmetafile","wmf"),
("application/x-msmoney","mny"),
("application/x-mspublisher","pub"),
("application/x-msschedule","scd"),
("application/x-msterminal","trm"),
("application/x-mswrite","wri"),
("application/x-netcdf","cdf"),
("application/x-netcdf","nc"),
("application/x-perfmon","pma"),
("application/x-perfmon","pmc"),
("application/x-perfmon","pml"),
("application/x-perfmon","pmr"),
("application/x-perfmon","pmw"),
("application/x-pkcs12","p12"),
("application/x-pkcs12","pfx"),
("application/x-pkcs7-certificates","p7b"),
("application/x-pkcs7-certificates","spc"),
("application/x-pkcs7-certreqresp","p7r"),
("application/x-pkcs7-mime","p7c"),
("application/x-pkcs7-mime","p7m"),
("application/x-pkcs7-signature","p7s"),
("application/x-sh","sh"),
("application/x-shar","shar"),
("application/x-shockwave-flash","swf"),
("application/x-stuffit","sit"),
("application/x-sv4cpio","sv4cpio"),
("application/x-sv4crc","sv4crc"),
("application/x-tar","tar"),
("application/x-tcl","tcl"),
("application/x-tex","tex"),
("application/x-texinfo","texi"),
("application/x-texinfo","texinfo"),
("application/x-troff","roff"),
("application/x-troff","t"),
("application/x-troff","tr"),
("application/x-troff-man","man"),
("application/x-troff-me","me"),
("application/x-troff-ms","ms"),
("application/x-ustar","ustar"),
("application/x-wais-source","src"),
("application/x-x509-ca-cert","cer"),
("application/x-x509-ca-cert","crt"),
("application/x-x509-ca-cert","der"),
("application/ynd.ms-pkipko","pko"),
("application/zip","zip"),
("audio/basic","au"),
("audio/basic","snd"),
("audio/mid","mid"),
("audio/mid","rmi"),
("audio/mpeg","mp3"),
("audio/x-aiff","aif"),
("audio/x-aiff","aifc"),
("audio/x-aiff","aiff"),
("audio/x-mpegurl","m3u"),
("audio/x-pn-realaudio","ra"),
("audio/x-pn-realaudio","ram"),
("audio/x-wav","wav"),
("image/bmp","bmp"),
("image/cis-cod","cod"),
("image/gif","gif"),
("image/ief","ief"),
("image/jpeg","jpe"),
("image/jpeg","jpeg"),
("image/jpeg","jpg"),
("image/pipeg","jfif"),
("image/svg+xml","svg"),
("image/tiff","tif"),
("image/tiff","tiff"),
("image/x-cmu-raster","ras"),
("image/x-cmx","cmx"),
("image/x-icon","ico"),
("image/x-portable-anymap","pnm"),
("image/x-portable-bitmap","pbm"),
("image/x-portable-graymap","pgm"),
("image/x-portable-pixmap","ppm"),
("image/x-rgb","rgb"),
("image/x-xbitmap","xbm"),
("image/x-xpixmap","xpm"),
("image/x-xwindowdump","xwd"),
("message/rfc822","mht"),
("message/rfc822","mhtml"),
("message/rfc822","nws"),
("text/css","css"),
("text/h323","323"),
("text/html","htm"),
("text/html","html"),
("text/html","stm"),
("text/iuls","uls"),
("text/plain","bas"),
("text/plain","c"),
("text/plain","h"),
("text/plain","txt"),
("text/richtext","rtx"),
("text/scriptlet","sct"),
("text/tab-separated-values","tsv"),
("text/webviewhtml","htt"),
("text/x-component","htc"),
("text/x-setext","etx"),
("text/x-vcard","vcf"),
("video/mpeg","mp2"),
("video/mpeg","mpa"),
("video/mpeg","mpe"),
("video/mpeg","mpeg"),
("video/mpeg","mpg"),
("video/mpeg","mpv2"),
("video/quicktime","mov"),
("video/quicktime","qt"),
("video/x-la-asf","lsf"),
("video/x-la-asf","lsx"),
("video/x-ms-asf","asf"),
("video/x-ms-asf","asr"),
("video/x-ms-asf","asx"),
("video/x-msvideo","avi"),
("video/x-sgi-movie","movie"),
("x-world/x-vrml","flr"),
("x-world/x-vrml","vrml"),
("x-world/x-vrml","wrl"),
("x-world/x-vrml","wrz"),
("x-world/x-vrml","xaf"),
("x-world/x-vrml","xof"),

        };

        //{后缀名，    文件类型}
        private static Dictionary<string, string> MATCH_ARRAY = new Dictionary<string, string> {
            {".3gp",    "video/3gpp"},
            {".apk",    "application/vnd.android.package-archive"},
            {".asf",    "video/x-ms-asf"},
            {".avi",    "video/x-msvideo"},
            {".bin",    "application/octet-stream"},
            {".bmp",      "image/bmp"},
            {".c",        "text/plain"},
            {".class",    "application/octet-stream"},
            {".conf",    "text/plain"},
            {".cpp",    "text/plain"},
            {".doc",    "application/msword"},
            {".exe",    "application/octet-stream"},
            {".gif",    "image/gif"},
            {".gtar",    "application/x-gtar"},
            {".gz",        "application/x-gzip"},
            {".h",        "text/plain"},
            {".htm",    "text/html"},
            {".html",    "text/html"},
            {".jar",    "application/java-archive"},
            {".java",    "text/plain"},
            {".jpeg",    "image/jpeg"},
            {".jpg",    "image/jpeg"},
            {".js",        "application/x-javascript"},
            {".log",    "text/plain"},
            {".m3u",    "audio/x-mpegurl"},
            {".m4a",    "audio/mp4a-latm"},
            {".m4b",    "audio/mp4a-latm"},
            {".m4p",    "audio/mp4a-latm"},
            {".m4u",    "video/vnd.mpegurl"},
            {".m4v",    "video/x-m4v"},
            {".mov",    "video/quicktime"},
            {".mp2",    "audio/x-mpeg"},
            {".mp3",    "audio/x-mpeg"},
            {".mp4",    "video/mp4"},
            {".mpc",    "application/vnd.mpohun.certificate"},
            {".mpe",    "video/mpeg"},
            {".mpeg",    "video/mpeg"},
            {".mpg",    "video/mpeg"},
            {".mpg4",    "video/mp4"},
            {".mpga",    "audio/mpeg"},
            {".msg",    "application/vnd.ms-outlook"},
            {".ogg",    "audio/ogg"},
            {".pdf",    "application/pdf"},
            {".png",    "image/png"},
            {".pps",    "application/vnd.ms-powerpoint"},
            {".ppt",    "application/vnd.ms-powerpoint"},
            {".prop",    "text/plain"},
            {".rar",    "application/x-rar-compressed"},
            {".rc",        "text/plain"},
            {".rmvb",    "audio/x-pn-realaudio"},
            {".rtf",    "application/rtf"},
            {".sh",        "text/plain"},
            {".tar",    "application/x-tar"},
            {".tgz",    "application/x-compressed"},
            {".txt",    "text/plain"},
            {".wav",    "audio/x-wav"},
            {".wma",    "audio/x-ms-wma"},
            {".wmv",    "audio/x-ms-wmv"},
            {".wps",    "application/vnd.ms-works"},
            {".xml",    "text/plain"},
            {".z",        "application/x-compress"},
            {".zip",    "application/zip"},
            {"",        "*/*"}
    };

    }
}