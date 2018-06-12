namespace LazyWelfare.AndroidMobile.Views.Partials
{
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.WebAgreement;

    public partial class UserEditView : IPartialView<UserModel>
    {
        public static AgreementUri Partial = new AgreementUri(nameof(UserEditView), nameof(PartialHost), typeof(UserEditView).Name);

        public void PushRequest(PartialActivity context, string args)
        {
            context.StackPush(Partial.Name, args);
        }

        public string GenerateString(string args)
        {
            this.Model = ActiveContext.User;
            return GenerateString();
        }
    }
}