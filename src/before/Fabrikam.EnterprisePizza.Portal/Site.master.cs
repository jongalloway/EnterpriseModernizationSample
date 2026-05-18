using System;
using Microsoft.AspNet.Identity;

namespace Fabrikam.EnterprisePizza.Portal
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var isAuthenticated = Context != null && Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated;
            AnonymousLinksPanel.Visible = !isAuthenticated;
            AuthenticatedLinksPanel.Visible = isAuthenticated;

            if (isAuthenticated)
            {
                WelcomeLiteral.Text = "Signed in as " + Server.HtmlEncode(Context.User.Identity.Name);
            }
        }

        protected void SignOutButton_Click(object sender, EventArgs e)
        {
            OwinContextAccessor.Create(Context).Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Response.Redirect("~/Default.aspx");
        }
    }
}
