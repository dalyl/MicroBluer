namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;
    using System.Collections.Generic;

    public partial class FolderMapsView : IPartialView<List<FolderMapModel>>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapsView), nameof(PartialHost), typeof(FolderMapsView).Name);

        public void PushRequest(PartialActivity context,string args) => context.RequestStack.Push(Partial.Name, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.FolderMapStore.GetList();
            return GenerateString();
        }
    }

   
}