using System;
using System.Data;

namespace Fabrikam.EnterprisePizza.Portal
{
    public partial class OperationsSnapshot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            var districtTable = new DataTable();
            districtTable.Columns.Add("District");
            districtTable.Columns.Add("OpenOrders");
            districtTable.Columns.Add("DriverCoverage");
            districtTable.Columns.Add("CateringCalls");

            districtTable.Rows.Add("North Metro", "18", "94%", "4");
            districtTable.Rows.Add("Airport Corridor", "11", "88%", "2");
            districtTable.Rows.Add("East Campus", "23", "91%", "5");

            DistrictGrid.DataSource = districtTable;
            DistrictGrid.DataBind();

            RouteNotesBullets.DataSource = new[]
            {
                "MapQuest export timed out for one suburban zone; dispatch is using last night's direction sheet.",
                "Two stores requested hand-updated mileage logs after printer ribbon failures.",
                "Weather advisory banner should stay posted through the dinner rush."
            };
            RouteNotesBullets.DataBind();

            PartnerQueueBullets.DataSource = new[]
            {
                "Adventure Works catering manager requested updated vegetarian tray pricing.",
                "Contoso Office Parks needs an extra tax-exempt certificate faxed before noon.",
                "South district franchisees still owe signed acknowledgement for the sauce cup inventory bulletin."
            };
            PartnerQueueBullets.DataBind();
        }
    }
}
