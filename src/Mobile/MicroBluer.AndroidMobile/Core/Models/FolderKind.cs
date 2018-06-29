using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MicroBluer.AndroidUtils;

namespace MicroBluer.AndroidMobile.Models
{
    public enum FolderKind
    {
        None = 0,
        Image,
        Video,
        Music,
        Other
    }

    public class FolderKindExtension
    {
        static readonly Dictionary<FolderKind, string[]> ExtensionDictionary = new Dictionary<FolderKind, string[]> {
                 { FolderKind.Image,new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" } },
                 { FolderKind.Video,new string[] { ".mp3", ".wma", ".wav", ".amr", ".m4a", ".m4r", ".ape", ".flac" } },
                 { FolderKind.Music,new string[] {  ".mp4",".rm",".rmvb",".mpeg1", ".mpeg2", ".mpeg3", ".mpeg4", ".3gp", ".flv" } },
        };

        public static FolderKind TryParse(string kindName)
        {
            var kind = FolderKind.None;
            if (Enum.TryParse(kindName, out kind) == false) return TryCatch.Current.Show(FolderKind.None, $"{kindName} 未定义的文件夹分类");
            return kind;
        }

        public static string[] GetExtensions(string kindName)
        {
            var kind = TryParse(kindName);
            if (kind == FolderKind.None) return TryCatch.Current.Show(new string[0], $"{kindName} 未定义的文件夹分类");
            return GetExtensions(kind);
        }

        public static string[] GetExtensions(FolderKind kind)
        {
            if (ExtensionDictionary.ContainsKey(kind) == false) return TryCatch.Current.Show(new string[0], $"{kind} 文件夹分类未指定后缀集");
            return ExtensionDictionary[kind];
        }
    }
}