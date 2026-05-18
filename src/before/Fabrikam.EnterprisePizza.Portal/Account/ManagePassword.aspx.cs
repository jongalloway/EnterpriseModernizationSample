using System;

namespace Fabrikam.EnterprisePizza.Portal.Account
{
    public partial class ManagePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                UserNameLiteral.Text = Server.HtmlEncode(Context.User.Identity.Name);
            }
        }

        protected void BackToAccountButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/Manage.aspx");
        }
    }
}
