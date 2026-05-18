<%@ Page Title="Inventory Management" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="InventoryManagement.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.InventoryManagement" %>
<asp:Content ID="InventoryHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="InventoryMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="InventoryScriptManager" runat="server" />
    <div class="pageIntro">
        <h1>Inventory management</h1>
        <p>Watch stock levels, reorder alerts, and supplier follow-up calls before the next truck window closes.</p>
    </div>
    <asp:UpdatePanel ID="InventoryUpdatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="pageLayout">
                <div class="pagePrimaryColumn">
                    <div class="legacyPanel">
                        <div class="toolbar">
                            <label for="<%= InventoryStoreSelector.ClientID %>">Store</label>
                            <asp:DropDownList ID="InventoryStoreSelector" runat="server" CssClass="legacySelect" DataSourceID="InventoryStoresDataSource" DataTextField="DisplayName" DataValueField="StoreNumber" AutoPostBack="true" />
                            <asp:Button ID="InventoryRefreshButton" runat="server" Text="Refresh Counts" CssClass="legacyButton" />
                            <span class="toolbarNote">Reorder flags are driven off the current par sheet and truck cadence.</span>
                        </div>
                        <asp:GridView ID="InventoryGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" DataSourceID="InventoryDataSource" DataKeyNames="Sku" GridLines="None">
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" SelectText="Supplier" />
                                <asp:BoundField HeaderText="SKU" DataField="Sku" />
                                <asp:BoundField HeaderText="Item" DataField="ItemName" />
                                <asp:BoundField HeaderText="Category" DataField="Category" />
                                <asp:BoundField HeaderText="On Hand" DataField="OnHand" />
                                <asp:BoundField HeaderText="Par" DataField="ParLevel" />
                                <asp:BoundField HeaderText="Reorder Point" DataField="ReorderPoint" />
                                <asp:BoundField HeaderText="Supplier" DataField="SupplierName" />
                                <asp:BoundField HeaderText="Alert" DataField="AlertLevel" />
                            </Columns>
                            <SelectedRowStyle CssClass="selectedRow" />
                        </asp:GridView>
                    </div>
                </div>
                <div class="pageSecondaryColumn">
                    <div class="sidebarPanel">
                        <h2>Supplier profile</h2>
                        <asp:DetailsView ID="SupplierDetailsView" runat="server" AutoGenerateRows="False" CssClass="legacyDetails" DataSourceID="SupplierDetailsDataSource" GridLines="Horizontal">
                            <Fields>
                                <asp:BoundField HeaderText="Supplier" DataField="SupplierName" />
                                <asp:BoundField HeaderText="Primary contact" DataField="PrimaryContact" />
                                <asp:BoundField HeaderText="Phone" DataField="ContactPhone" />
                                <asp:BoundField HeaderText="Delivery window" DataField="DeliveryWindow" />
                                <asp:BoundField HeaderText="Minimum order" DataField="MinimumOrder" />
                                <asp:BoundField HeaderText="Backup supplier" DataField="BackupSupplier" />
                                <asp:BoundField HeaderText="Recommendation" DataField="ReorderRecommendation" />
                            </Fields>
                        </asp:DetailsView>
                    </div>
                    <div class="sidebarPanel sidebarPanelMuted">
                        <h2>Count note</h2>
                        <p>Critical packaging items should be escalated before 6:00 PM so the district runner can still squeeze in a stop.</p>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="InventoryStoresDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetStores" />
    <asp:ObjectDataSource ID="InventoryDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetInventoryItems">
        <SelectParameters>
            <asp:ControlParameter ControlID="InventoryStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SupplierDetailsDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetSupplierProfile">
        <SelectParameters>
            <asp:ControlParameter ControlID="InventoryStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="InventoryGrid" Name="sku" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
