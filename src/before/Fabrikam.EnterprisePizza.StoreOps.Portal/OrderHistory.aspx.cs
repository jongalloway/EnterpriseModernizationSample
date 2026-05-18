using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fabrikam.EnterprisePizza.StoreOps.Portal.Data;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal
{
    public partial class OrderHistory : Page
    {
        private const string StoreFilterViewStateKey = "OrderHistory.Store";
        private const string StartDateFilterViewStateKey = "OrderHistory.StartDate";
        private const string EndDateFilterViewStateKey = "OrderHistory.EndDate";
        private const string ServiceModeFilterViewStateKey = "OrderHistory.ServiceMode";
        private const string SortExpressionViewStateKey = "OrderHistory.Sort";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            BindStoreFilter();
            BindServiceModeFilter();
            ResetFilterControls();
            SaveFilterState();
            BindHistoryGrid("History packet view loaded.");
        }

        protected void ApplyHistoryButton_Click(object sender, EventArgs e)
        {
            SaveFilterState();
            OrderHistoryGrid.PageIndex = 0;
            BindHistoryGrid("History filter refreshed.");
        }

        protected void ResetHistoryButton_Click(object sender, EventArgs e)
        {
            ResetFilterControls();
            SaveFilterState();
            OrderHistoryGrid.PageIndex = 0;
            BindHistoryGrid("History filter reset to the nightly binder defaults.");
        }

        protected void OrderHistoryGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            OrderHistoryGrid.PageIndex = e.NewPageIndex;
            BindHistoryGrid("Moved to history page " + (e.NewPageIndex + 1) + ".");
        }

        protected void OrderHistoryGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState[SortExpressionViewStateKey] = ResolveSortExpression(e.SortExpression);
            OrderHistoryGrid.PageIndex = 0;
            BindHistoryGrid("History sorted by " + e.SortExpression + ".");
        }

        protected void OrderHistoryDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["storeNumber"] = GetFilterValue(StoreFilterViewStateKey, GetDefaultStoreNumber());
            e.InputParameters["startDateText"] = GetFilterValue(StartDateFilterViewStateKey, DateTime.Today.AddDays(-14).ToString("MM/dd/yyyy"));
            e.InputParameters["endDateText"] = GetFilterValue(EndDateFilterViewStateKey, DateTime.Today.ToString("MM/dd/yyyy"));
            e.InputParameters["serviceModeFilter"] = GetFilterValue(ServiceModeFilterViewStateKey, "All");
            e.InputParameters["sortExpression"] = GetFilterValue(SortExpressionViewStateKey, "BusinessDate DESC");
        }

        private void BindHistoryGrid(string statusMessage)
        {
            OrderHistoryGrid.DataBind();
            var rows = new OrderManagementLegacyDataSource().GetOrderHistory(
                GetFilterValue(StoreFilterViewStateKey, GetDefaultStoreNumber()),
                GetFilterValue(StartDateFilterViewStateKey, DateTime.Today.AddDays(-14).ToString("MM/dd/yyyy")),
                GetFilterValue(EndDateFilterViewStateKey, DateTime.Today.ToString("MM/dd/yyyy")),
                GetFilterValue(ServiceModeFilterViewStateKey, "All"),
                GetFilterValue(SortExpressionViewStateKey, "BusinessDate DESC"));

            OrderHistorySummaryLiteral.Text = string.Format("<span class=\"summaryCallout\">{0} packets in range</span>", rows.Count);
            OrderHistoryStatusLabel.Text = statusMessage + " Last refresh " + DateTime.Now.ToString("g") + ".";
        }

        private void BindStoreFilter()
        {
            if (HistoryStoreFilter.Items.Count > 0)
            {
                return;
            }

            HistoryStoreFilter.Items.Add(new ListItem("014 - Downtown Dispatch", "014"));
            HistoryStoreFilter.Items.Add(new ListItem("022 - Midtown", "022"));
            HistoryStoreFilter.Items.Add(new ListItem("031 - Mall Annex", "031"));
            HistoryStoreFilter.Items.Add(new ListItem("057 - Airport Service Road", "057"));
            HistoryStoreFilter.Items.Add(new ListItem("081 - College Commons", "081"));
        }

        private void BindServiceModeFilter()
        {
            if (HistoryServiceModeFilter.Items.Count > 0)
            {
                return;
            }

            HistoryServiceModeFilter.Items.Add("All");
            HistoryServiceModeFilter.Items.Add("Delivery");
            HistoryServiceModeFilter.Items.Add("Carryout");
        }

        private void ResetFilterControls()
        {
            HistoryStoreFilter.SelectedValue = GetDefaultStoreNumber();
            HistoryStartDateTextBox.Text = DateTime.Today.AddDays(-14).ToString("MM/dd/yyyy");
            HistoryEndDateTextBox.Text = DateTime.Today.ToString("MM/dd/yyyy");
            HistoryServiceModeFilter.SelectedValue = "All";
            ViewState[SortExpressionViewStateKey] = "BusinessDate DESC";
        }

        private void SaveFilterState()
        {
            ViewState[StoreFilterViewStateKey] = HistoryStoreFilter.SelectedValue;
            ViewState[StartDateFilterViewStateKey] = HistoryStartDateTextBox.Text.Trim();
            ViewState[EndDateFilterViewStateKey] = HistoryEndDateTextBox.Text.Trim();
            ViewState[ServiceModeFilterViewStateKey] = HistoryServiceModeFilter.SelectedValue;

            if (ViewState[SortExpressionViewStateKey] == null)
            {
                ViewState[SortExpressionViewStateKey] = "BusinessDate DESC";
            }
        }

        private string ResolveSortExpression(string requestedSortExpression)
        {
            var currentSortExpression = GetFilterValue(SortExpressionViewStateKey, "BusinessDate DESC");
            if (currentSortExpression.Equals(requestedSortExpression, StringComparison.OrdinalIgnoreCase))
            {
                return requestedSortExpression + " DESC";
            }

            if (currentSortExpression.Equals(requestedSortExpression + " DESC", StringComparison.OrdinalIgnoreCase))
            {
                return requestedSortExpression;
            }

            return requestedSortExpression;
        }

        private string GetFilterValue(string viewStateKey, string defaultValue)
        {
            return ViewState[viewStateKey] as string ?? defaultValue;
        }

        private static string GetDefaultStoreNumber()
        {
            return ConfigurationManager.AppSettings["DefaultStoreNumber"] ?? "014";
        }
    }
}
