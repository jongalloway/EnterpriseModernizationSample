<%@ Page Title="Account Center" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.Account.Manage" %>
<asp:Content ID="ManageMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Account Center</h2>
        <p>Review the active legacy identity profile and the next steps still managed by the partner portal help desk.</p>
    </div>
    <div class="account-shell">
        <div class="module">
            <h3>Current profile</h3>
            <div class="account-body">
                <table class="account-facts">
                    <tr>
                        <td>User name</td>
                        <td><asp:Literal ID="UserNameLiteral" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Email</td>
                        <td><asp:Literal ID="EmailLiteral" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Authentication</td>
                        <td>ASP.NET Identity 2.2.3 application cookie</td>
                    </tr>
                </table>
                <asp:Button ID="ContinueButton" runat="server" Text="Return to rollups" CssClass="secondary-button" OnClick="ContinueButton_Click" />
            </div>
        </div>
        <div class="module">
            <h3>Pending self-service items</h3>
            <div class="account-body">
                <ul class="account-list">
                    <li>Role assignment still follows the legacy HQ approval chain and remains outside this portal page.</li>
                    <li>Password rotation and reset workflows route through the security stub until the broader identity retrofit is complete.</li>
                    <li>Additional profile fields can be layered in later without replacing the Web Forms shell.</li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
