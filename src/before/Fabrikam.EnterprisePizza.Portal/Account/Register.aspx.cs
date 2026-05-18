using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Fabrikam.EnterprisePizza.Portal.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Manage.aspx");
            }
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var owinContext = OwinContextAccessor.Create(Context);
            var manager = owinContext.GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser
            {
                UserName = UserNameTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim()
            };

            var result = manager.Create(user, PasswordTextBox.Text);
            if (result.Succeeded)
            {
                var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                owinContext.Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                Response.Redirect("~/Account/Manage.aspx");
                return;
            }

            FailureMessageLabel.Text = string.Join("<br />", result.Errors.Select(Server.HtmlEncode));
            FailureMessageLabel.Visible = true;
        }
    }
}
