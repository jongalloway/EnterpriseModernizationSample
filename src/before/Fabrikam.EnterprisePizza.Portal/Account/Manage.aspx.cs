using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Fabrikam.EnterprisePizza.Portal.Account
{
    public partial class Manage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnsureAuthenticated();
            if (IsPostBack)
            {
                return;
            }

            var manager = OwinContextAccessor.Create(Context).GetUserManager<ApplicationUserManager>();
            var user = manager.FindById(Context.User.Identity.GetUserId());
            if (user == null)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            UserNameLiteral.Text = Server.HtmlEncode(user.UserName);
            EmailLiteral.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(user.Email) ? "Not captured" : user.Email);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/OperationsSnapshot.aspx");
        }

        private void EnsureAuthenticated()
        {
            if (Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
            }
        }
    }
}
