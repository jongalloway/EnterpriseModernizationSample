<%@ Page Title="Store Settings" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="StoreSettings.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.StoreSettings" %>
<asp:Content ID="SettingsHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="SettingsMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="SettingsScriptManager" runat="server" />
    <div class="pageIntro">
        <h1>Store settings</h1>
        <p>Review operating hours, delivery-zone rules, and staffing caps from the same Web Forms shell managers have been patching for years.</p>
    </div>
    <asp:UpdatePanel ID="SettingsUpdatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="pageLayout">
                <div class="pagePrimaryColumn">
                    <div class="legacyPanel">
                        <div class="toolbar">
                            <label for="<%= SettingsStoreSelector.ClientID %>">Store</label>
                            <asp:DropDownList ID="SettingsStoreSelector" runat="server" CssClass="legacySelect" DataSourceID="SettingsStoresDataSource" DataTextField="DisplayName" DataValueField="StoreNumber" AutoPostBack="true" />
                            <asp:Button ID="SettingsRefreshButton" runat="server" Text="Reload Settings" CssClass="legacyButton" />
                            <span class="toolbarNote">Update requests still flow through district approvals and end-of-night print packets.</span>
                        </div>
                        <asp:DetailsView ID="StoreSettingsView" runat="server" AutoGenerateRows="False" CssClass="legacyDetails" DataSourceID="StoreConfigurationDataSource" GridLines="Horizontal">
                            <Fields>
                                <asp:BoundField HeaderText="Store number" DataField="StoreNumber" />
                                <asp:BoundField HeaderText="Store name" DataField="StoreName" />
                                <asp:BoundField HeaderText="District" DataField="District" />
                                <asp:BoundField HeaderText="Weekday hours" DataField="WeekdayHours" />
                                <asp:BoundField HeaderText="Weekend hours" DataField="WeekendHours" />
                                <asp:BoundField HeaderText="Delivery radius" DataField="DeliveryRadiusMiles" />
                                <asp:BoundField HeaderText="Dispatch mode" DataField="DispatchMode" />
                                <asp:BoundField HeaderText="Driver cap" DataField="DriverCap" />
                                <asp:BoundField HeaderText="Make-line cap" DataField="MakeLineCap" />
                                <asp:BoundField HeaderText="Counter cap" DataField="CounterCap" />
                                <asp:BoundField HeaderText="Last manager review" DataField="LastManagerReview" />
                            </Fields>
                        </asp:DetailsView>
                    </div>
                </div>
                <div class="pageSecondaryColumn">
                    <div class="sidebarPanel">
                        <h2>Delivery zones</h2>
                        <asp:GridView ID="DeliveryZoneGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" DataSourceID="DeliveryZonesDataSource" GridLines="None">
                            <Columns>
                                <asp:BoundField HeaderText="Zone" DataField="ZoneCode" />
                                <asp:BoundField HeaderText="Coverage" DataField="ZoneName" />
                                <asp:BoundField HeaderText="Miles" DataField="RadiusMiles" />
                                <asp:BoundField HeaderText="Dispatch rule" DataField="DispatchRule" />
                                <asp:BoundField HeaderText="Driver cap" DataField="DriverCap" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="sidebarPanel sidebarPanelMuted">
                        <h2>Manager note</h2>
                        <p>Zone-radius changes still travel on paper sign-off after the supervisor prints this page for the district binder.</p>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="SettingsStoresDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetStores" />
    <asp:ObjectDataSource ID="StoreConfigurationDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetStoreConfiguration">
        <SelectParameters>
            <asp:ControlParameter ControlID="SettingsStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="DeliveryZonesDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetDeliveryZones">
        <SelectParameters>
            <asp:ControlParameter ControlID="SettingsStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
