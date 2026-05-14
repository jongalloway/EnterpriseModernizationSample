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
        <div class="module">
            <h3>Route Notes</h3>
            <asp:BulletedList ID="RouteNotesBullets" runat="server" CssClass="checklist" />
        </div>
        <div class="module">
            <h3>Partner Services Queue</h3>
            <asp:BulletedList ID="PartnerQueueBullets" runat="server" CssClass="checklist" />
        </div>
    </div>
</asp:Content>
