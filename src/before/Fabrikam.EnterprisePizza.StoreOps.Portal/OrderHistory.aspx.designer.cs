namespace Fabrikam.EnterprisePizza.StoreOps.Portal
{
    public partial class OrderHistory
    {
        protected global::System.Web.UI.WebControls.DropDownList HistoryStoreFilter;

        protected global::System.Web.UI.WebControls.TextBox HistoryStartDateTextBox;
 
        protected global::AjaxControlToolkit.CalendarExtender HistoryStartDateCalendar;
 
        protected global::System.Web.UI.WebControls.DropDownList HistoryServiceModeFilter;

        protected global::System.Web.UI.WebControls.TextBox HistoryEndDateTextBox;
 
        protected global::AjaxControlToolkit.CalendarExtender HistoryEndDateCalendar;
 
        protected global::System.Web.UI.WebControls.Button ApplyHistoryButton;

        protected global::System.Web.UI.WebControls.Button ResetHistoryButton;

        protected global::System.Web.UI.WebControls.Label OrderHistoryStatusLabel;

        protected global::System.Web.UI.WebControls.Literal OrderHistorySummaryLiteral;

        protected global::System.Web.UI.WebControls.GridView OrderHistoryGrid;

        protected global::System.Web.UI.WebControls.ObjectDataSource OrderHistoryDataSource;
    }
}
