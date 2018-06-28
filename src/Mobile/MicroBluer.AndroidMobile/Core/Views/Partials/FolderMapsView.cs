namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;
    using System.Collections.Generic;

    public partial class FolderMapsView : IPartialView<List<FolderMapModel>>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapsView), nameof(PartialHost), typeof(FolderMapsView).Name);

        public void PushRequest(PartialActivity context,string args) => context.StackPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.FolderMapStore.GetList();
            return GenerateString();
        }
    }

   
}