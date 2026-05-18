using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fabrikam.EnterprisePizza.StoreOps.Portal.Data;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal
{
    public partial class OrderDetails : Page
    {
        private const string OrderNumberViewStateKey = "OrderDetails.OrderNumber";
        private const string StoreNumberViewStateKey = "OrderDetails.StoreNumber";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            LoadRouteState();
            BindOrderDetails("Order details loaded from the release sheet.");
        }

        protected void RefreshDetailsButton_Click(object sender, EventArgs e)
        {
            BindOrderDetails("Order details refreshed.");
        }

        protected void OrderDetailsDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["orderNumber"] = ResolveOrderNumber();
            e.InputParameters["storeNumber"] = ResolveStoreNumber();
        }

        protected void OrderTimelineDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["orderNumber"] = ResolveOrderNumber();
            e.InputParameters["storeNumber"] = ResolveStoreNumber();
        }

        private void BindOrderDetails(string statusMessage)
        {
            var dataSource = new OrderManagementLegacyDataSource();
            var details = dataSource.GetOrderDetails(ResolveOrderNumber(), ResolveStoreNumber());

            OrderDetailsView.DataBind();
            OrderTimelineGrid.DataBind();
            OrderDetailsStatusLabel.Text = statusMessage + " Last refresh " + DateTime.Now.ToString("g") + ".";
            OrderHeaderLiteral.Text = details.Count == 0
                ? "<span class=\"summaryCallout\">No ticket in scope</span>"
                : string.Format("<span class=\"summaryCallout\">Store {0} / ticket {1}</span>", details[0].StoreNumber, details[0].OrderNumber);

            BindFollowUpList(details);
        }

        private void BindFollowUpList(IList<OrderDetailRecord> details)
        {
            OrderFollowUpList.Items.Clear();
            if (details.Count == 0)
            {
                OrderFollowUpList.Items.Add("No follow-up card is available for the requested order.");
                return;
            }

            var detail = details[0];
            OrderFollowUpList.Items.Add("Confirm " + detail.CustomerName + " at the " + detail.ClerkStation + " station log.");
            OrderFollowUpList.Items.Add(detail.FollowUpNote);
            OrderFollowUpList.Items.Add("Release through " + detail.DriverOrCounter + " after the supervisor initials the packet.");
        }

        private void LoadRouteState()
        {
            var requestedStoreNumber = Request.QueryString["storeNumber"];
            var requestedOrderNumber = Request.QueryString["orderNumber"];
            int parsedOrderNumber;

            ViewState[StoreNumberViewStateKey] = string.IsNullOrWhiteSpace(requestedStoreNumber)
                ? GetDefaultStoreNumber()
                : requestedStoreNumber.Trim();

            if (!int.TryParse(requestedOrderNumber, out parsedOrderNumber))
            {
                parsedOrderNumber = new OrderManagementLegacyDataSource().GetLookupOrders(GetDefaultStoreNumber(), string.Empty, "All", "All", "OrderNumber")[0].OrderNumber;
            }

            ViewState[OrderNumberViewStateKey] = parsedOrderNumber;
        }

        private int ResolveOrderNumber()
        {
            return ViewState[OrderNumberViewStateKey] == null ? 0 : (int)ViewState[OrderNumberViewStateKey];
        }

        private string ResolveStoreNumber()
        {
            return ViewState[StoreNumberViewStateKey] as string ?? GetDefaultStoreNumber();
        }

        private static string GetDefaultStoreNumber()
        {
            return ConfigurationManager.AppSettings["DefaultStoreNumber"] ?? "014";
        }
    }
}
