namespace MicroBluer.AndroidMobile.Views.Partials
{
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.WebAgreement;

    public partial class UserEditView : IPartialView<UserModel>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(UserEditView), nameof(PartialHost), typeof(UserEditView).Name);

        public void PushRequest(PartialActivity context, string args) => context.StackPush(Partial, args);

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.User;
            return GenerateString();
        }
    }
}