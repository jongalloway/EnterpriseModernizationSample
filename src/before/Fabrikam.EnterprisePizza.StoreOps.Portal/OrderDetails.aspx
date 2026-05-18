<%@ Page Title="Order Details" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.OrderDetails" %>
<asp:Content ID="OrderDetailsHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="OrderDetailsMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageHero pageHeroOrders">
        <div class="heroCopyWide">
            <h1>Single-order release sheet</h1>
            <p>
                The details page keeps the kitchen note, payment hold, and release timeline together so supervisors do not have to flip between the counter binder and dispatch screen.
            </p>
        </div>
        <div class="heroBadge heroBadgeCompact">
            <span class="badgeLabel">Legacy flow</span>
            <span class="badgeValue">DetailsView + status grid</span>
        </div>
    </div>

    <div class="dashboardLayout">
        <div class="dashboardMain">
            <div class="legacyPanel detailToolbarPanel">
                <div class="toolbar toolbarCompact">
                    <asp:Button ID="RefreshDetailsButton" runat="server" Text="Refresh Ticket" CssClass="legacyButton" OnClick="RefreshDetailsButton_Click" />
                    <asp:LinkButton ID="ReleaseChecklistLink" runat="server" CssClass="legacyButtonLink">Review release checklist</asp:LinkButton>
                    <a href="OrderLookup.aspx" class="legacyButtonLink">Back to lookup</a>
                    <a href="OrderHistory.aspx" class="legacyButtonLink">Open history</a>
                    <asp:Label ID="OrderDetailsStatusLabel" runat="server" CssClass="toolbarNote statusInlineLabel" />
                </div>
            </div>

            <div class="legacyPanel">
                <div class="panelHeaderRow">
                    <h2>Order jacket summary</h2>
                    <asp:Literal ID="OrderHeaderLiteral" runat="server" />
                </div>
                <asp:DetailsView ID="OrderDetailsView" runat="server" AutoGenerateRows="False" CssClass="legacyDetailsView" GridLines="None"
                    DataSourceID="OrderDetailsDataSource" EmptyDataText="No order is available for the requested ticket.">
                    <Fields>
                        <asp:BoundField HeaderText="Order #" DataField="OrderNumber" />
                        <asp:BoundField HeaderText="Store" DataField="StoreNumber" />
                        <asp:BoundField HeaderText="Guest" DataField="CustomerName" />
                        <asp:BoundField HeaderText="Business date" DataField="BusinessDate" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                        <asp:BoundField HeaderText="Channel" DataField="Channel" />
                        <asp:BoundField HeaderText="Service mode" DataField="ServiceMode" />
                        <asp:BoundField HeaderText="Promise window" DataField="PromiseWindow" />
                        <asp:BoundField HeaderText="Ticket total" DataField="TicketTotal" DataFormatString="{0:C}" HtmlEncode="false" />
                        <asp:BoundField HeaderText="Kitchen status" DataField="KitchenStatus" />
                        <asp:BoundField HeaderText="Dispatch status" DataField="DispatchStatus" />
                        <asp:BoundField HeaderText="Payment status" DataField="PaymentStatus" />
                        <asp:BoundField HeaderText="Clerk station" DataField="ClerkStation" />
                        <asp:BoundField HeaderText="Release lane" DataField="DriverOrCounter" />
                        <asp:BoundField HeaderText="Last touch" DataField="LastTouchDisplay" />
                        <asp:BoundField HeaderText="Issue card" DataField="IssueFlag" />
                        <asp:BoundField HeaderText="Follow-up note" DataField="FollowUpNote" />
                    </Fields>
                </asp:DetailsView>
                <asp:ObjectDataSource ID="OrderDetailsDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Data.OrderManagementLegacyDataSource" SelectMethod="GetOrderDetails" OnSelecting="OrderDetailsDataSource_Selecting" />
            </div>

            <div class="legacyPanel gridPanel">
                <h2>Status timeline</h2>
                <asp:GridView ID="OrderTimelineGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" GridLines="None"
                    DataSourceID="OrderTimelineDataSource" EmptyDataText="No timeline entries are available for this order.">
                    <Columns>
                        <asp:BoundField HeaderText="Time" DataField="EventTime" />
                        <asp:BoundField HeaderText="Step" DataField="Step" />
                        <asp:BoundField HeaderText="Station" DataField="Station" />
                        <asp:BoundField HeaderText="Status" DataField="Status" />
                        <asp:BoundField HeaderText="Note" DataField="Note" />
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="OrderTimelineDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Data.OrderManagementLegacyDataSource" SelectMethod="GetOrderStatusTimeline" OnSelecting="OrderTimelineDataSource_Selecting" />
            </div>
        </div>

        <div class="dashboardSidebar">
            <div class="sidebarPanel">
                <h2>Counter follow-up card</h2>
                <asp:BulletedList ID="OrderFollowUpList" runat="server" CssClass="sidebarBulletList" />
            </div>
            <div class="sidebarPanel sidebarPanelMuted">
                <h2>Paper packet reminder</h2>
                <p>Staple the exception card to the nightly packet before the shift manager signs the close sheet.</p>
            </div>
        </div>
    </div>

    <asp:Panel ID="ReleaseChecklistPanel" runat="server" CssClass="modalPanel" Style="display: none;">
        <div class="modalHeader">Release checklist</div>
        <ul class="sidebarBulletList modalBulletList">
            <li>Match the callback initials against the issue card before release.</li>
            <li>Confirm the payment hold is cleared on the counter printer tape.</li>
            <li>Log the handoff station before the shift lead signs the packet.</li>
        </ul>
        <asp:Button ID="CloseReleaseChecklistButton" runat="server" Text="Close" CssClass="legacyButton" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="ReleaseChecklistPopup" runat="server" TargetControlID="ReleaseChecklistLink"
        PopupControlID="ReleaseChecklistPanel" CancelControlID="CloseReleaseChecklistButton" BackgroundCssClass="modalBackdrop" />
</asp:Content>
