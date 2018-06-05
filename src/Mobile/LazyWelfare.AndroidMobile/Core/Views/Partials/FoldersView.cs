namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using LazyWelfare.AndroidMobile.Models;
    using System.Collections.Generic;

    public partial class FoldersView : IPartialView<List<FolderMapModel>>
    {
        public static (string Host, string Path) Partial = (nameof(PartialView), typeof(FoldersView).Name);

        public string GenerateStringWithoutModel() => GenerateString();

        public List<FolderMapModel> GetModel(string args)
        {
            return ActiveContext.FolderStore.GetList();
        }
    }
}