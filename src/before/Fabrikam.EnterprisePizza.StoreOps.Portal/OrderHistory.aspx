<%@ Page Title="Order History" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.OrderHistory" %>
<asp:Content ID="OrderHistoryHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="OrderHistoryMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageHero pageHeroOrders">
        <div class="heroCopyWide">
            <h1>Nightly order history binder</h1>
            <p>
                Review closed delivery and carryout packets by business date when accounting, store managers, or franchise support ask for the old paper trail.
            </p>
        </div>
        <div class="heroBadge heroBadgeCompact">
            <span class="badgeLabel">Packet window</span>
            <span class="badgeValue">Prior 2 weeks by default</span>
        </div>
    </div>

    <div class="dashboardLayout">
        <div class="dashboardMain">
            <div class="legacyPanel filterPanel">
                <h2>History filters</h2>
                <table class="filterTable" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="filterLabel"><label for="HistoryStoreFilter">Store</label></td>
                        <td><asp:DropDownList ID="HistoryStoreFilter" runat="server" CssClass="legacySelect" /></td>
                        <td class="filterLabel"><label for="HistoryStartDateTextBox">Start date</label></td>
                        <td><asp:TextBox ID="HistoryStartDateTextBox" runat="server" CssClass="legacyTextBox legacyDateTextBox" /></td>
                    </tr>
                    <tr>
                        <td class="filterLabel"><label for="HistoryServiceModeFilter">Service mode</label></td>
                        <td><asp:DropDownList ID="HistoryServiceModeFilter" runat="server" CssClass="legacySelect" /></td>
                        <td class="filterLabel"><label for="HistoryEndDateTextBox">End date</label></td>
                        <td><asp:TextBox ID="HistoryEndDateTextBox" runat="server" CssClass="legacyTextBox legacyDateTextBox" /></td>
                    </tr>
                </table>
                <div class="toolbar toolbarCompact">
                    <asp:Button ID="ApplyHistoryButton" runat="server" Text="Apply History Filter" CssClass="legacyButton" OnClick="ApplyHistoryButton_Click" />
                    <asp:Button ID="ResetHistoryButton" runat="server" Text="Reset" CssClass="legacyButton" CausesValidation="false" OnClick="ResetHistoryButton_Click" />
                    <asp:Label ID="OrderHistoryStatusLabel" runat="server" CssClass="toolbarNote statusInlineLabel" />
                </div>
            </div>

            <div class="legacyPanel gridPanel">
                <div class="panelHeaderRow">
                    <h2>Closed order packets</h2>
                    <asp:Literal ID="OrderHistorySummaryLiteral" runat="server" />
                </div>
                <asp:GridView ID="OrderHistoryGrid" runat="server" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="true" PageSize="10"
                    CssClass="legacyGrid" GridLines="None" DataSourceID="OrderHistoryDataSource" OnPageIndexChanging="OrderHistoryGrid_PageIndexChanging"
                    OnSorting="OrderHistoryGrid_Sorting" EmptyDataText="No order packets are available for the selected dates.">
                    <Columns>
                        <asp:BoundField HeaderText="Business date" DataField="BusinessDateDisplay" SortExpression="BusinessDate" />
                        <asp:HyperLinkField HeaderText="Order #" DataNavigateUrlFields="OrderNumber,StoreNumber" DataNavigateUrlFormatString="~/OrderDetails.aspx?orderNumber={0}&amp;storeNumber={1}" DataTextField="OrderNumber" SortExpression="OrderNumber" />
                        <asp:BoundField HeaderText="Guest" DataField="CustomerName" SortExpression="CustomerName" />
                        <asp:BoundField HeaderText="Channel" DataField="Channel" />
                        <asp:BoundField HeaderText="Service" DataField="ServiceMode" SortExpression="ServiceMode" />
                        <asp:BoundField HeaderText="Close status" DataField="CloseStatus" SortExpression="CloseStatus" />
                        <asp:BoundField HeaderText="Settlement" DataField="SettlementStatus" />
                        <asp:BoundField HeaderText="Total" DataField="TicketTotal" SortExpression="TicketTotal" DataFormatString="{0:C}" HtmlEncode="false" />
                        <asp:BoundField HeaderText="Night note" DataField="CloseNote" />
                    </Columns>
                    <PagerStyle CssClass="legacyPager" />
                </asp:GridView>
                <asp:ObjectDataSource ID="OrderHistoryDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Data.OrderManagementLegacyDataSource" SelectMethod="GetOrderHistory" OnSelecting="OrderHistoryDataSource_Selecting" />
            </div>
        </div>

        <div class="dashboardSidebar">
            <div class="sidebarPanel">
                <h2>History habits</h2>
                <ul class="sidebarBulletList">
                    <li>Keep the date range short when the shift manager is waiting on the phone.</li>
                    <li>Use <strong>Close status</strong> to find callbacks and remake packets first.</li>
                    <li>Open the details page before filing a reprint request.</li>
                </ul>
            </div>
            <div class="sidebarPanel sidebarPanelMuted">
                <h2>Current orders</h2>
                <p>Jump back to the active lookup when the desk needs tonight's jacket instead of a closed packet.</p>
                <a href="OrderLookup.aspx" class="legacyButtonLink">Return to lookup</a>
            </div>
        </div>
    </div>
</asp:Content>
