<%@ Page Title="Order Lookup" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="OrderLookup.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.OrderLookup" %>
<asp:Content ID="OrderLookupHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="OrderLookupMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageHero pageHeroOrders">
        <div class="heroCopyWide">
            <h1>Order lookup binder</h1>
            <p>
                Search by guest, order number, or service mode, then drill into the same release notes the shift lead still keeps beside the counter printer.
            </p>
        </div>
        <div class="heroBadge heroBadgeCompact">
            <span class="badgeLabel">Supervisor note</span>
            <span class="badgeValue">Use after counter callbacks</span>
        </div>
    </div>

    <div class="dashboardLayout">
        <div class="dashboardMain">
            <div class="legacyPanel filterPanel">
                <h2>Lookup filters</h2>
                <table class="filterTable" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="filterLabel"><label for="LookupStoreFilter">Store</label></td>
                        <td><asp:DropDownList ID="LookupStoreFilter" runat="server" CssClass="legacySelect" /></td>
                        <td class="filterLabel"><label for="LookupSearchTextBox">Guest / order</label></td>
                        <td>
                            <asp:TextBox ID="LookupSearchTextBox" runat="server" CssClass="legacyTextBox" MaxLength="40" />
                            <ajaxToolkit:AutoCompleteExtender ID="LookupSearchAutoComplete" runat="server" TargetControlID="LookupSearchTextBox"
                                ServiceMethod="GetLookupSuggestions" MinimumPrefixLength="2" CompletionInterval="200" CompletionSetCount="8"
                                CompletionListCssClass="legacyAutoCompleteList" CompletionListItemCssClass="legacyAutoCompleteItem"
                                CompletionListHighlightedItemCssClass="legacyAutoCompleteItemSelected" />
                            <span class="fieldHint">Type two letters or ticket digits for ready-made suggestions.</span>
                        </td>
                    </tr>
                    <tr>
                        <td class="filterLabel"><label for="LookupStatusFilter">Status</label></td>
                        <td><asp:DropDownList ID="LookupStatusFilter" runat="server" CssClass="legacySelect" /></td>
                        <td class="filterLabel"><label for="LookupServiceModeFilter">Service mode</label></td>
                        <td><asp:DropDownList ID="LookupServiceModeFilter" runat="server" CssClass="legacySelect" /></td>
                    </tr>
                </table>
                <div class="toolbar toolbarCompact">
                    <asp:Button ID="ApplyLookupButton" runat="server" Text="Apply Lookup" CssClass="legacyButton" OnClick="ApplyLookupButton_Click" />
                    <asp:Button ID="ResetLookupButton" runat="server" Text="Reset" CssClass="legacyButton" CausesValidation="false" OnClick="ResetLookupButton_Click" />
                    <asp:Label ID="LookupStatusLabel" runat="server" CssClass="toolbarNote statusInlineLabel" />
                </div>
            </div>

            <div class="legacyPanel gridPanel">
                <div class="panelHeaderRow">
                    <h2>Active order jackets</h2>
                    <asp:Literal ID="LookupSummaryLiteral" runat="server" />
                </div>
                <asp:GridView ID="OrderLookupGrid" runat="server" AutoGenerateColumns="False" AllowPaging="true" AllowSorting="true" PageSize="8"
                    CssClass="legacyGrid" GridLines="None" DataSourceID="OrderLookupDataSource" OnPageIndexChanging="OrderLookupGrid_PageIndexChanging"
                    OnSorting="OrderLookupGrid_Sorting" EmptyDataText="No orders match the current lookup.">
                    <Columns>
                        <asp:HyperLinkField HeaderText="Order #" DataNavigateUrlFields="OrderNumber,StoreNumber" DataNavigateUrlFormatString="~/OrderDetails.aspx?orderNumber={0}&amp;storeNumber={1}" DataTextField="OrderNumber" SortExpression="OrderNumber" />
                        <asp:BoundField HeaderText="Customer" DataField="CustomerName" SortExpression="CustomerName" />
                        <asp:BoundField HeaderText="Channel" DataField="Channel" />
                        <asp:BoundField HeaderText="Service" DataField="ServiceMode" SortExpression="ServiceMode" />
                        <asp:BoundField HeaderText="Promise" DataField="PromiseWindow" SortExpression="PromiseWindow" />
                        <asp:BoundField HeaderText="Kitchen" DataField="KitchenStatus" />
                        <asp:BoundField HeaderText="Release" DataField="DispatchStatus" SortExpression="DispatchStatus" />
                        <asp:BoundField HeaderText="Payment" DataField="PaymentStatus" SortExpression="PaymentStatus" />
                        <asp:BoundField HeaderText="Last touch" DataField="LastTouchDisplay" />
                        <asp:BoundField HeaderText="Issue card" DataField="IssueFlag" />
                        <asp:BoundField HeaderText="Total" DataField="TicketTotal" SortExpression="TicketTotal" DataFormatString="{0:C}" HtmlEncode="false" />
                    </Columns>
                    <PagerStyle CssClass="legacyPager" />
                    <HeaderStyle CssClass="legacyGridHeader" />
                </asp:GridView>
                <asp:ObjectDataSource ID="OrderLookupDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Data.OrderManagementLegacyDataSource" SelectMethod="GetLookupOrders" OnSelecting="OrderLookupDataSource_Selecting" />
            </div>
        </div>

        <div class="dashboardSidebar">
            <div class="sidebarPanel">
                <h2>Lookup habits</h2>
                <ul class="sidebarBulletList">
                    <li>Search by office name before calling the store back line.</li>
                    <li>Watch <strong>Carryout Hold</strong> and <strong>Exception</strong> orders first.</li>
                    <li>Open the details page before updating the paper callback log.</li>
                </ul>
            </div>
            <div class="sidebarPanel sidebarPanelMuted">
                <h2>Need closed orders?</h2>
                <p>Use the nightly binder view when a manager asks for prior-day close packets or callback history.</p>
                <a href="OrderHistory.aspx" class="legacyButtonLink">Open order history</a>
            </div>
        </div>
    </div>
</asp:Content>
