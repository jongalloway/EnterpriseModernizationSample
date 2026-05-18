<%@ Page Title="Dispatch Board" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="DispatchBoard.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.DispatchBoard" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="DispatchHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="DispatchMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="DispatchScriptManager" runat="server" />
    <div class="pageIntro">
        <h1>Dispatch board</h1>
        <p>Monitor active runs, driver availability, and timer-based refreshes from the same supervisor shell used during dinner rush.</p>
    </div>
    <asp:UpdatePanel ID="DispatchUpdatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="pageLayout">
                <div class="pagePrimaryColumn">
                    <div class="legacyPanel">
                        <div class="toolbar">
                            <label for="<%= DispatchStoreSelector.ClientID %>">Store</label>
                            <asp:DropDownList ID="DispatchStoreSelector" runat="server" CssClass="legacySelect" DataSourceID="DispatchStoresDataSource" DataTextField="DisplayName" DataValueField="StoreNumber" AutoPostBack="true" />
                            <asp:Button ID="DispatchRefreshButton" runat="server" Text="Refresh Board" CssClass="legacyButton" />
                            <span class="toolbarNote">The board refreshes every 45 seconds for the wall monitor.</span>
                        </div>
                        <asp:Timer ID="DispatchRefreshTimer" runat="server" Interval="45000" />
                        <asp:GridView ID="DispatchGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" DataSourceID="DispatchBoardDataSource" DataKeyNames="DriverCode" GridLines="None" EmptyDataText="No active delivery tickets are staged right now.">
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" SelectText="Driver" />
                                <asp:BoundField HeaderText="Ticket" DataField="TicketId" />
                                <asp:BoundField HeaderText="Customer" DataField="CustomerName" />
                                <asp:BoundField HeaderText="Driver" DataField="DriverCode" />
                                <asp:BoundField HeaderText="Zone" DataField="RouteZone" />
                                <asp:BoundField HeaderText="State" DataField="DispatchState" />
                                <asp:BoundField HeaderText="Driver Status" DataField="DriverStatus" />
                                <asp:BoundField HeaderText="Minutes Open" DataField="MinutesOpen" />
                                <asp:BoundField HeaderText="Promise" DataField="PromiseWindow" />
                                <asp:BoundField HeaderText="Ready At" DataField="ReadyAt" />
                            </Columns>
                            <SelectedRowStyle CssClass="selectedRow" />
                        </asp:GridView>
                    </div>
                </div>
                <div class="pageSecondaryColumn">
                    <div class="sidebarPanel">
                        <h2>Driver status</h2>
                        <asp:DetailsView ID="DriverStatusView" runat="server" AutoGenerateRows="False" CssClass="legacyDetails" DataSourceID="DriverStatusDataSource" GridLines="Horizontal">
                            <Fields>
                                <asp:BoundField HeaderText="Driver code" DataField="DriverCode" />
                                <asp:BoundField HeaderText="Driver name" DataField="DriverName" />
                                <asp:BoundField HeaderText="Status" DataField="Status" />
                                <asp:BoundField HeaderText="Current zone" DataField="CurrentZone" />
                                <asp:BoundField HeaderText="Active runs" DataField="ActiveRuns" />
                                <asp:BoundField HeaderText="Last check-in" DataField="LastCheckIn" />
                                <asp:BoundField HeaderText="Next availability" DataField="NextAvailability" />
                                <asp:BoundField HeaderText="Vehicle" DataField="AssignedVehicle" />
                                <asp:BoundField HeaderText="Manager note" DataField="ManagerNote" />
                            </Fields>
                        </asp:DetailsView>
                    </div>
                    <div class="sidebarPanel sidebarPanelMuted">
                        <h2>Rush reminder</h2>
                        <p>Keep the desktop dispatch board on the second monitor, then use this web board for quick driver lookups and reroute notes.</p>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="DispatchStoresDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetStores" />
    <asp:ObjectDataSource ID="DispatchBoardDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetDispatchBoard">
        <SelectParameters>
            <asp:ControlParameter ControlID="DispatchStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="DriverStatusDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetDriverStatus">
        <SelectParameters>
            <asp:ControlParameter ControlID="DispatchStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="DispatchGrid" Name="driverCode" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
