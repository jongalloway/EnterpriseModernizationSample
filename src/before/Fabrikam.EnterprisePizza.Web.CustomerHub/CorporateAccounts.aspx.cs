using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Web.CustomerHub
{
    public partial class CorporateAccounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            var table = new DataTable();
            table.Columns.Add("Partner");
            table.Rows.Add("Contoso Office Parks");
            table.Rows.Add("Adventure Works Bike Expo");
            PartnerGrid.DataSource = table;
            PartnerGrid.DataBind();
        }
    }
}
