<%@ Page Title="Staff Schedule" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="StaffSchedule.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal.StaffSchedule" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="ScheduleHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="ScheduleMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScheduleScriptManager" runat="server" />
    <div class="pageIntro">
        <h1>Staff schedule</h1>
        <p>Review the weekly staffing grid, pick a schedule week, and keep overtime watch items visible for supervisors.</p>
    </div>
    <asp:UpdatePanel ID="ScheduleUpdatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="pageLayout pageLayoutWide">
                <div class="pageWideColumn">
                    <div class="legacyPanel">
                        <div class="toolbar toolbarWithCalendar">
                            <label for="<%= ScheduleStoreSelector.ClientID %>">Store</label>
                            <asp:DropDownList ID="ScheduleStoreSelector" runat="server" CssClass="legacySelect" DataSourceID="ScheduleStoresDataSource" DataTextField="DisplayName" DataValueField="StoreNumber" AutoPostBack="true" />
                            <label for="<%= WeekOfTextBox.ClientID %>">Week of</label>
                            <asp:TextBox ID="WeekOfTextBox" runat="server" CssClass="legacyTextBox" Text="05/18/2026" AutoPostBack="true" />
                            <ajaxToolkit:CalendarExtender ID="WeekOfCalendarExtender" runat="server" TargetControlID="WeekOfTextBox" Format="MM/dd/yyyy" />
                            <asp:Button ID="ScheduleRefreshButton" runat="server" Text="Refresh Schedule" CssClass="legacyButton" />
                        </div>
                        <asp:GridView ID="ScheduleGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid legacyGridCompact" DataSourceID="ScheduleDataSource" DataKeyNames="EmployeeCode" GridLines="None">
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" SelectText="Detail" />
                                <asp:BoundField HeaderText="Employee" DataField="EmployeeName" />
                                <asp:BoundField HeaderText="Role" DataField="Role" />
                                <asp:BoundField HeaderText="Mon" DataField="Monday" />
                                <asp:BoundField HeaderText="Tue" DataField="Tuesday" />
                                <asp:BoundField HeaderText="Wed" DataField="Wednesday" />
                                <asp:BoundField HeaderText="Thu" DataField="Thursday" />
                                <asp:BoundField HeaderText="Fri" DataField="Friday" />
                                <asp:BoundField HeaderText="Sat" DataField="Saturday" />
                                <asp:BoundField HeaderText="Sun" DataField="Sunday" />
                                <asp:BoundField HeaderText="Hours" DataField="WeeklyHours" />
                                <asp:BoundField HeaderText="Certification" DataField="Certification" />
                            </Columns>
                            <SelectedRowStyle CssClass="selectedRow" />
                        </asp:GridView>
                    </div>
                </div>
                <div class="pageSecondaryColumn">
                    <div class="sidebarPanel">
                        <h2>Coverage summary</h2>
                        <asp:DetailsView ID="CoverageSummaryView" runat="server" AutoGenerateRows="False" CssClass="legacyDetails" DataSourceID="CoverageSummaryDataSource" GridLines="Horizontal">
                            <Fields>
                                <asp:BoundField HeaderText="Employee" DataField="EmployeeName" />
                                <asp:BoundField HeaderText="Role" DataField="Role" />
                                <asp:BoundField HeaderText="Week of" DataField="WeekOf" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField HeaderText="Hours" DataField="WeeklyHours" />
                                <asp:BoundField HeaderText="Certification" DataField="Certification" />
                                <asp:BoundField HeaderText="Coverage note" DataField="CoverageNote" />
                                <asp:BoundField HeaderText="Manager note" DataField="ManagerNote" />
                            </Fields>
                        </asp:DetailsView>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="ScheduleStoresDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetStores" />
    <asp:ObjectDataSource ID="ScheduleDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetWeeklySchedule">
        <SelectParameters>
            <asp:ControlParameter ControlID="ScheduleStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="WeekOfTextBox" Name="weekOf" PropertyName="Text" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="CoverageSummaryDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.StoreOps.Portal.Services.StoreOperationsPortalService" SelectMethod="GetShiftCoverage">
        <SelectParameters>
            <asp:ControlParameter ControlID="ScheduleStoreSelector" Name="storeNumber" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="WeekOfTextBox" Name="weekOf" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="ScheduleGrid" Name="employeeCode" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
