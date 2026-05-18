<%@ Page Title="Partner Dashboard" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="PartnerDashboard.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.PartnerDashboard" %>
<asp:Content ID="HeadPartnerDashboard" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyPartnerDashboard" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Partner Dashboard</h2>
        <p>One dashboard for application intake, contract attention items, and referral channels feeding the summer launch plan.</p>
    </div>

    <asp:FormView ID="DashboardSummaryFormView" runat="server" DataSourceID="DashboardSummarySource" RenderOuterTable="false">
        <ItemTemplate>
            <div class="metric-board">
                <div class="metric-card">
                    <strong>Active partners</strong>
                    <span><%# Eval("ActivePartners") %></span>
                </div>
                <div class="metric-card">
                    <strong>Pending applications</strong>
                    <span><%# Eval("PendingApplications") %></span>
                </div>
                <div class="metric-card">
                    <strong>Contracts awaiting action</strong>
                    <span><%# Eval("ContractsAwaitingAction") %></span>
                </div>
                <div class="metric-card">
                    <strong>Referral commissions</strong>
                    <span><%# Eval("MonthlyReferralCommissions", "{0:C}") %></span>
                </div>
                <div class="metric-card accent-card">
                    <strong>Franchise launches in queue</strong>
                    <span><%# Eval("FranchiseLaunches") %></span>
                </div>
            </div>
        </ItemTemplate>
    </asp:FormView>

    <div class="column-wrap">
        <div class="module">
            <h3>Pending applications</h3>
            <asp:GridView ID="PendingApplicationsGrid" runat="server" CssClass="portal-grid compact-grid" DataSourceID="PendingApplicationsSource" AutoGenerateColumns="false" EmptyDataText="No pending partner applications are waiting in the binder.">
                <Columns>
                    <asp:BoundField DataField="CompanyName" HeaderText="Company" />
                    <asp:BoundField DataField="ReferralChannel" HeaderText="Channel" />
                    <asp:BoundField DataField="TargetLaunchDate" HeaderText="Launch" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="module">
            <h3>Contract watch list</h3>
            <asp:GridView ID="ContractWatchGrid" runat="server" CssClass="portal-grid compact-grid" DataSourceID="ContractWatchSource" AutoGenerateColumns="false" EmptyDataText="No contracts currently need extra attention.">
                <Columns>
                    <asp:BoundField DataField="PartnerName" HeaderText="Partner" />
                    <asp:BoundField DataField="AgreementType" HeaderText="Agreement" />
                    <asp:BoundField DataField="RenewalDate" HeaderText="Renewal" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="module">
        <h3>Referral leaderboard</h3>
        <asp:GridView ID="ReferralLeadersGrid" runat="server" CssClass="portal-grid compact-grid" DataSourceID="ReferralLeadersSource" AutoGenerateColumns="false" EmptyDataText="No referral commissions have been posted yet.">
            <Columns>
                <asp:BoundField DataField="PartnerName" HeaderText="Partner" />
                <asp:BoundField DataField="QualifiedReferrals" HeaderText="Qualified referrals" />
                <asp:BoundField DataField="CommissionEarned" HeaderText="Commission earned" DataFormatString="{0:C}" HtmlEncode="false" />
            </Columns>
        </asp:GridView>
    </div>

    <asp:ObjectDataSource ID="DashboardSummarySource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetDashboardSummary" />
    <asp:ObjectDataSource ID="PendingApplicationsSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetPendingApplications" />
    <asp:ObjectDataSource ID="ContractWatchSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetContractWatchList" />
    <asp:ObjectDataSource ID="ReferralLeadersSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetReferralLeaders" />
</asp:Content>
