using System;
using Microsoft.AspNet.Identity.Owin;

namespace Fabrikam.EnterprisePizza.Portal.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Manage.aspx");
            }
        }

        protected void SignInButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var signInManager = OwinContextAccessor.Create(Context).Get<ApplicationSignInManager>();
            var status = signInManager.PasswordSignInAsync(UserNameTextBox.Text.Trim(), PasswordTextBox.Text, RememberMeCheckBox.Checked, shouldLockout: false).GetAwaiter().GetResult();

            switch (status)
            {
                case SignInStatus.Success:
                    RedirectToRequestedUrl();
                    break;
                case SignInStatus.LockedOut:
                    ShowFailure("This legacy account is locked. Call the partner portal help desk to be re-enabled.");
                    break;
                default:
                    ShowFailure("The user name or password was not recognized. Double-check the legacy credentials and try again.");
                    break;
            }
        }

        private void RedirectToRequestedUrl()
        {
            var returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith("/", StringComparison.Ordinal) && !returnUrl.StartsWith("//", StringComparison.Ordinal) && !returnUrl.StartsWith("/\\", StringComparison.Ordinal))
            {
                Response.Redirect(returnUrl);
                return;
            }

            Response.Redirect("~/Account/Manage.aspx");
        }

        private void ShowFailure(string message)
        {
            FailureMessageLabel.Text = message;
            FailureMessageLabel.Visible = true;
        }
    }
}
