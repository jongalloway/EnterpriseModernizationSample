using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fabrikam.EnterprisePizza.StoreOps.Portal.Data;

namespace Fabrikam.EnterprisePizza.StoreOps.Portal
{
    public partial class OrderLookup : Page
    {
        private static readonly string[] LookupStores = { "014", "022", "031", "057", "081" };

        private const string StoreFilterViewStateKey = "OrderLookup.Store";
        private const string SearchFilterViewStateKey = "OrderLookup.Search";
        private const string StatusFilterViewStateKey = "OrderLookup.Status";
        private const string ServiceModeFilterViewStateKey = "OrderLookup.ServiceMode";
        private const string SortExpressionViewStateKey = "OrderLookup.Sort";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            BindStoreFilter();
            BindStatusFilter();
            BindServiceModeFilter();
            ResetFilterControls();
            ApplyQueryStringDefaults();
            SaveFilterState();
            BindLookupGrid("Lookup loaded from the supervisor binder.");
        }

        protected void ApplyLookupButton_Click(object sender, EventArgs e)
        {
            SaveFilterState();
            OrderLookupGrid.PageIndex = 0;
            BindLookupGrid("Lookup refreshed for the current clerk filters.");
        }

        protected void ResetLookupButton_Click(object sender, EventArgs e)
        {
            ResetFilterControls();
            SaveFilterState();
            OrderLookupGrid.PageIndex = 0;
            BindLookupGrid("Lookup reset to the default dinner-rush view.");
        }

        protected void OrderLookupGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            OrderLookupGrid.PageIndex = e.NewPageIndex;
            BindLookupGrid("Moved to lookup page " + (e.NewPageIndex + 1) + ".");
        }

        protected void OrderLookupGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState[SortExpressionViewStateKey] = ResolveSortExpression(e.SortExpression);
            OrderLookupGrid.PageIndex = 0;
            BindLookupGrid("Lookup sorted by " + e.SortExpression + ".");
        }

        protected void OrderLookupDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["storeNumber"] = GetFilterValue(StoreFilterViewStateKey, GetDefaultStoreNumber());
            e.InputParameters["searchText"] = GetFilterValue(SearchFilterViewStateKey, string.Empty);
            e.InputParameters["statusFilter"] = GetFilterValue(StatusFilterViewStateKey, "All");
            e.InputParameters["serviceModeFilter"] = GetFilterValue(ServiceModeFilterViewStateKey, "All");
            e.InputParameters["sortExpression"] = GetFilterValue(SortExpressionViewStateKey, "PromiseWindow");
        }

        [WebMethod]
        [ScriptMethod]
        public static string[] GetLookupSuggestions(string prefixText, int count)
        {
            if (string.IsNullOrWhiteSpace(prefixText))
            {
                return new string[0];
            }

            var normalizedPrefix = prefixText.Trim();
            var suggestionCount = count <= 0 ? 8 : count;
            var dataSource = new OrderManagementLegacyDataSource();

            return LookupStores
                .SelectMany(storeNumber => dataSource.GetLookupOrders(storeNumber, string.Empty, "All", "All", "OrderNumber"))
                .SelectMany(row => new[]
                {
                    row.CustomerName,
                    row.OrderNumber.ToString(CultureInfo.InvariantCulture),
                    row.CustomerName + " (" + row.OrderNumber.ToString(CultureInfo.InvariantCulture) + ")"
                })
                .Where(value => value.IndexOf(normalizedPrefix, StringComparison.OrdinalIgnoreCase) >= 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(suggestionCount)
                .ToArray();
        }

        private void BindLookupGrid(string statusMessage)
        {
            OrderLookupGrid.DataBind();

            var rows = new OrderManagementLegacyDataSource().GetLookupOrders(
                GetFilterValue(StoreFilterViewStateKey, GetDefaultStoreNumber()),
                GetFilterValue(SearchFilterViewStateKey, string.Empty),
                GetFilterValue(StatusFilterViewStateKey, "All"),
                GetFilterValue(ServiceModeFilterViewStateKey, "All"),
                GetFilterValue(SortExpressionViewStateKey, "PromiseWindow"));

            LookupSummaryLiteral.Text = string.Format("<span class=\"summaryCallout\">{0} visible / {1} exception cards</span>", rows.Count, CountExceptionRows(rows));
            LookupStatusLabel.Text = statusMessage + " Last refresh " + DateTime.Now.ToString("g") + ".";
        }

        private void BindStoreFilter()
        {
            if (LookupStoreFilter.Items.Count > 0)
            {
                return;
            }

            LookupStoreFilter.Items.Add(new ListItem("014 - Downtown Dispatch", "014"));
            LookupStoreFilter.Items.Add(new ListItem("022 - Midtown", "022"));
            LookupStoreFilter.Items.Add(new ListItem("031 - Mall Annex", "031"));
            LookupStoreFilter.Items.Add(new ListItem("057 - Airport Service Road", "057"));
            LookupStoreFilter.Items.Add(new ListItem("081 - College Commons", "081"));
        }

        private void BindStatusFilter()
        {
            if (LookupStatusFilter.Items.Count > 0)
            {
                return;
            }

            LookupStatusFilter.Items.Add("All");
            LookupStatusFilter.Items.Add("Ready");
            LookupStatusFilter.Items.Add("Staged");
            LookupStatusFilter.Items.Add("Carryout Hold");
            LookupStatusFilter.Items.Add("Exception");
            LookupStatusFilter.Items.Add("Settled");
            LookupStatusFilter.Items.Add("Cash Pending");
            LookupStatusFilter.Items.Add("Card Hold");
            LookupStatusFilter.Items.Add("Needs callback");
            LookupStatusFilter.Items.Add("Kitchen remake");
            LookupStatusFilter.Items.Add("Payment follow-up");
        }

        private void BindServiceModeFilter()
        {
            if (LookupServiceModeFilter.Items.Count > 0)
            {
                return;
            }

            LookupServiceModeFilter.Items.Add("All");
            LookupServiceModeFilter.Items.Add("Delivery");
            LookupServiceModeFilter.Items.Add("Carryout");
        }

        private void ResetFilterControls()
        {
            LookupStoreFilter.SelectedValue = GetDefaultStoreNumber();
            LookupSearchTextBox.Text = string.Empty;
            LookupStatusFilter.SelectedValue = "All";
            LookupServiceModeFilter.SelectedValue = "All";
            ViewState[SortExpressionViewStateKey] = "PromiseWindow";
        }

        private void ApplyQueryStringDefaults()
        {
            var storeNumber = Request.QueryString["storeNumber"];
            if (!string.IsNullOrWhiteSpace(storeNumber) && LookupStoreFilter.Items.FindByValue(storeNumber) != null)
            {
                LookupStoreFilter.SelectedValue = storeNumber;
            }

            var searchText = Request.QueryString["search"];
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                LookupSearchTextBox.Text = searchText.Trim();
            }
        }

        private void SaveFilterState()
        {
            ViewState[StoreFilterViewStateKey] = LookupStoreFilter.SelectedValue;
            ViewState[SearchFilterViewStateKey] = LookupSearchTextBox.Text.Trim();
            ViewState[StatusFilterViewStateKey] = LookupStatusFilter.SelectedValue;
            ViewState[ServiceModeFilterViewStateKey] = LookupServiceModeFilter.SelectedValue;

            if (ViewState[SortExpressionViewStateKey] == null)
            {
                ViewState[SortExpressionViewStateKey] = "PromiseWindow";
            }
        }

        private string ResolveSortExpression(string requestedSortExpression)
        {
            var currentSortExpression = GetFilterValue(SortExpressionViewStateKey, "PromiseWindow");
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

        private static string GetDefaultStoreNumber()
        {
            return ConfigurationManager.AppSettings["DefaultStoreNumber"] ?? "014";
        }

        private string GetFilterValue(string viewStateKey, string defaultValue)
        {
            return ViewState[viewStateKey] as string ?? defaultValue;
        }

        private static int CountExceptionRows(System.Collections.Generic.IEnumerable<OrderLookupRow> rows)
        {
            var count = 0;
            foreach (var row in rows)
            {
                if (!row.IssueFlag.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
