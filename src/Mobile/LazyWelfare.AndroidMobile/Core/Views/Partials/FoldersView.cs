namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;
    using System.Collections.Generic;

    public partial class FoldersView : IPartialView<List<FolderMapModel>>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FoldersView), nameof(PartialHost), typeof(FoldersView).Name);

        public void PushRequest(PartialActivity context,string args) => context.RequestStack.Push(Partial.Name, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.FolderStore.GetList();
            return GenerateString();
        }
    }

   
}