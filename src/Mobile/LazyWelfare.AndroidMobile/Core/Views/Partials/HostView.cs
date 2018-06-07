namespace LazyWelfare.AndroidMobile.Views.Partials
{

    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class HostView : IPartialView<HostModel>
    {

        public static AgreementUri Partial = new AgreementUri(nameof(HostView), nameof(PartialHost), typeof(HostView).Name);

        public void PushRequest(PartialActivity context, string args)
        {
            context.RequestStack.Clear();
            context.RequestStack.Push(Partial.Name, args);
        }

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.Host;
            return GenerateString();
        }

    }
}