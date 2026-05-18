using System;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace Fabrikam.EnterprisePizza.Portal
{
    public partial class PartnerSignup : System.Web.UI.Page
    {
        protected void SignupFormView_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                return;
            }

            SuccessPanel.Visible = true;
            StatusMessageLabel.Text = "Application routed to the regional partner binder and queued for morning review.";
        }

        [WebMethod]
        [ScriptMethod]
        public static string[] SearchPartners(string prefixText, int count)
        {
            return new PartnerManagementRepository().SearchPartnerNames(prefixText, count);
        }
    }
}
