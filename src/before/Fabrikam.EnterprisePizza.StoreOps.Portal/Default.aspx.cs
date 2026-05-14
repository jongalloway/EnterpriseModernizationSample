using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal
{
    public partial class _Default : Page
    {
        private readonly DispatchCoordinator _dispatchCoordinator = new DispatchCoordinator();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            BindStores();
            BindDashboard();
        }

        protected void RefreshBoardButton_Click(object sender, EventArgs e)
        {
            BindDashboard();
        }

        private void BindStores()
        {
            if (StoreSelector.Items.Count > 0)
            {
                return;
            }

            StoreSelector.Items.Add("014");
            StoreSelector.Items.Add("022");
            StoreSelector.Items.Add("031");
            StoreSelector.SelectedValue = GetDefaultStoreNumber();
        }

        private void BindDashboard()
        {
            var storeNumber = string.IsNullOrWhiteSpace(StoreSelector.SelectedValue)
                ? GetDefaultStoreNumber()
                : StoreSelector.SelectedValue;

            SelectedStoreNumber.Value = storeNumber;
            ActiveRoutesGrid.DataSource = BuildDispatchRows(_dispatchCoordinator.GetActiveTickets(storeNumber));
            ActiveRoutesGrid.DataBind();

            LaborWatchGrid.DataSource = BuildLaborWatchRows(storeNumber);
            LaborWatchGrid.DataBind();

            ZoneBulletinList.DataSource = BuildZoneBulletins(storeNumber);
            ZoneBulletinList.DataBind();
        }

        private static string GetDefaultStoreNumber()
        {
            return ConfigurationManager.AppSettings["DefaultStoreNumber"] ?? "014";
        }

        private static IList<DispatchBoardRow> BuildDispatchRows(IEnumerable<DispatchTicket> tickets)
        {
            return tickets.Select((ticket, index) => new DispatchBoardRow
            {
                TicketId = ticket.TicketId,
                DriverCode = ticket.DriverCode,
                RouteZone = ticket.RouteZone,
                DispatchState = index == 0 ? "Out for delivery" : "Queued for reroute review",
                MinutesOpen = index == 0 ? 14 : 9
            }).ToList();
        }

        private static IList<LaborWatchRow> BuildLaborWatchRows(string storeNumber)
        {
            return new List<LaborWatchRow>
            {
                new LaborWatchRow
                {
                    StoreNumber = storeNumber,
                    TeamName = "Drivers",
                    Concern = "2 drivers approaching overtime threshold",
                    ActionRequired = "Shift meal-break coverage before 7:00 PM"
                },
                new LaborWatchRow
                {
                    StoreNumber = storeNumber,
                    TeamName = "Make line",
                    Concern = "Cross-trained cashier pulled to prep",
                    ActionRequired = "Supervisor sign-off pending on labor transfer"
                },
                new LaborWatchRow
                {
                    StoreNumber = storeNumber,
                    TeamName = "Front counter",
                    Concern = "One call-out logged after lunch",
                    ActionRequired = "Hold flex labor unless queue exceeds 8 tickets"
                }
            };
        }

        private static IList<string> BuildZoneBulletins(string storeNumber)
        {
            return new List<string>
            {
                "Store " + storeNumber + " northwest corridor: watch apartment gate codes after 6:30 PM.",
                "Mall annex route: security desk still prefers paper receipt slips for after-hours drop-offs.",
                "Corporate park run: keep insulated bags on hand for stacked catering warm-holds."
            };
        }

        private class DispatchBoardRow
        {
            public int TicketId { get; set; }

            public string DriverCode { get; set; }

            public string RouteZone { get; set; }

            public string DispatchState { get; set; }

            public int MinutesOpen { get; set; }
        }

        private class LaborWatchRow
        {
            public string StoreNumber { get; set; }

            public string TeamName { get; set; }

            public string Concern { get; set; }

            public string ActionRequired { get; set; }
        }
    }
}
