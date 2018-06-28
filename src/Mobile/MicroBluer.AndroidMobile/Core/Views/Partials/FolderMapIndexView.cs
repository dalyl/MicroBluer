namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.WebAgreement;

    public partial  class FolderMapIndexView: IPartialView
    {
        public static AgreementUri Partial = new AgreementUri(nameof(FolderMapIndexView), nameof(PartialHost), typeof(FolderMapIndexView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackClearPush(Partial, args);

        public string GenerateString(string args)
        {
            return GenerateString();
        }
    }
}