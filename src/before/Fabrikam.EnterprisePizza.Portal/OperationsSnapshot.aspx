<%@ Page Title="Operations Snapshot" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="OperationsSnapshot.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.OperationsSnapshot" %>
<asp:Content ID="HeadOps" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyOps" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>District Operations Snapshot</h2>
        <p>Daily rollup used by field support before the lunch rush conference call.</p>
    </div>

    <div class="module">
        <h3>District Readiness</h3>
        <asp:GridView ID="DistrictGrid" runat="server" AutoGenerateColumns="false" CssClass="portal-grid">
            <Columns>
                <asp:BoundField DataField="District" HeaderText="District" />
                <asp:BoundField DataField="OpenOrders" HeaderText="Open Orders" />
                <asp:BoundField DataField="DriverCoverage" HeaderText="Driver Coverage" />
                <asp:BoundField DataField="CateringCalls" HeaderText="Catering Calls" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="column-wrap">
        <div class="module collapsibleModule">
            <asp:Panel ID="RouteNotesHeaderPanel" runat="server" CssClass="module-toggle">
                <h3>Route Notes</h3>
                <span class="module-toggle-state" id="RouteNotesToggleStateLabel" runat="server">Hide route notes</span>
            </asp:Panel>
            <asp:Panel ID="RouteNotesBodyPanel" runat="server" CssClass="module-body">
                <asp:BulletedList ID="RouteNotesBullets" runat="server" CssClass="checklist" />
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="RouteNotesCollapsible" runat="server" TargetControlID="RouteNotesBodyPanel"
                ExpandControlID="RouteNotesHeaderPanel" CollapseControlID="RouteNotesHeaderPanel" TextLabelID="RouteNotesToggleStateLabel"
                CollapsedText="Show route notes" ExpandedText="Hide route notes" Collapsed="false" />
        </div>
        <div class="module collapsibleModule">
            <asp:Panel ID="PartnerQueueHeaderPanel" runat="server" CssClass="module-toggle">
                <h3>Partner Services Queue</h3>
                <span class="module-toggle-state" id="PartnerQueueToggleStateLabel" runat="server">Hide queue details</span>
            </asp:Panel>
            <asp:Panel ID="PartnerQueueBodyPanel" runat="server" CssClass="module-body">
                <asp:BulletedList ID="PartnerQueueBullets" runat="server" CssClass="checklist" />
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="PartnerQueueCollapsible" runat="server" TargetControlID="PartnerQueueBodyPanel"
                ExpandControlID="PartnerQueueHeaderPanel" CollapseControlID="PartnerQueueHeaderPanel" TextLabelID="PartnerQueueToggleStateLabel"
                CollapsedText="Show queue details" ExpandedText="Hide queue details" Collapsed="true" />
        </div>
    </div>
</asp:Content>
