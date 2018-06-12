namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using System;
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class FolderMapEditView : IPartialView<FolderMapModel>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapsView), nameof(PartialHost), typeof(FolderMapEditView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial.Name, args);

        public string GenerateString(string args)
        {
            this.Model = string.IsNullOrEmpty(args) ? new FolderMapModel { Guid = Guid.NewGuid() } : ActiveContext.FolderMapStore.Get(args);
            return GenerateString();
        }
    }
}