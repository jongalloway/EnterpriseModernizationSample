<%@ Page Title="Referral Tracking" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ReferralTracking.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.ReferralTracking" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeadReferralTracking" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyReferralTracking" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Referral Tracking</h2>
        <p>Track attribution, booked orders, and commission checks for the partner channels that feed catering demand.</p>
    </div>

    <div class="module">
        <h3>Referral summary</h3>
        <div class="module-body filter-bar compact-filter-bar">
            <div class="filter-field">
                <label for="ReferralPartnerSearchTextBox">Partner name</label>
                <asp:TextBox ID="ReferralPartnerSearchTextBox" runat="server" Width="220" />
                <ajaxToolkit:AutoCompleteExtender ID="ReferralPartnerAutoComplete" runat="server" TargetControlID="ReferralPartnerSearchTextBox" ServiceMethod="SearchPartners" MinimumPrefixLength="1" CompletionSetCount="8" CompletionInterval="150" EnableCaching="true" />
            </div>
            <div class="filter-field">
                <label for="ReferralStatusFilter">Status</label>
                <asp:DropDownList ID="ReferralStatusFilter" runat="server">
                    <asp:ListItem Text="All statuses" Value="All" />
                    <asp:ListItem Text="Qualified" Value="Qualified" />
                    <asp:ListItem Text="Pending attribution" Value="Pending attribution" />
                    <asp:ListItem Text="Paid" Value="Paid" />
                </asp:DropDownList>
            </div>
            <div class="filter-actions">
                <asp:Button ID="ApplyReferralFiltersButton" runat="server" Text="Refresh Totals" CssClass="portal-button" />
            </div>
        </div>
        <asp:FormView ID="ReferralSummaryFormView" runat="server" DataSourceID="ReferralSummarySource" RenderOuterTable="false">
            <ItemTemplate>
                <div class="summary-strip">
                    <div class="summary-pill">
                        <strong>Total referrals</strong>
                        <span><%# Eval("TotalReferrals") %></span>
                    </div>
                    <div class="summary-pill">
                        <strong>Qualified</strong>
                        <span><%# Eval("QualifiedReferrals") %></span>
                    </div>
                    <div class="summary-pill">
                        <strong>Commission earned</strong>
                        <span><%# Eval("CommissionEarned", "{0:C}") %></span>
                    </div>
                    <div class="summary-pill">
                        <strong>Average payout</strong>
                        <span><%# Eval("AverageCommission", "{0:C}") %></span>
                    </div>
                </div>
            </ItemTemplate>
        </asp:FormView>
    </div>

    <div class="module">
        <h3>Referral ledger</h3>
        <asp:GridView ID="ReferralGrid" runat="server" CssClass="portal-grid" DataSourceID="ReferralDataSource" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true" PageSize="6" EmptyDataText="No referral activity matched the current filters.">
            <Columns>
                <asp:BoundField DataField="ReferralId" HeaderText="Referral #" SortExpression="ReferralId" />
                <asp:BoundField DataField="PartnerName" HeaderText="Partner" SortExpression="PartnerName" />
                <asp:BoundField DataField="ReferralChannel" HeaderText="Channel" SortExpression="ReferralChannel" />
                <asp:BoundField DataField="CustomerName" HeaderText="Customer" SortExpression="CustomerName" />
                <asp:BoundField DataField="AttributedOn" HeaderText="Attributed" SortExpression="AttributedOn" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="OrdersBooked" HeaderText="Orders" SortExpression="OrdersBooked" />
                <asp:BoundField DataField="CommissionEarned" HeaderText="Commission" SortExpression="CommissionEarned" DataFormatString="{0:C}" HtmlEncode="false" />
                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            </Columns>
        </asp:GridView>
    </div>

    <asp:ObjectDataSource ID="ReferralSummarySource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetReferralSummary">
        <SelectParameters>
            <asp:ControlParameter ControlID="ReferralPartnerSearchTextBox" Name="partnerName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ReferralStatusFilter" Name="status" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="ReferralDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetReferrals" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="ReferralPartnerSearchTextBox" Name="partnerName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ReferralStatusFilter" Name="status" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
