<%@ Page Title="Security Stub" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ManagePassword.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.Account.ManagePassword" %>
<asp:Content ID="ManagePasswordMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Security and Password Stub</h2>
        <p>Classic password management now hangs off the Identity user store, but the self-service screens still need a later pass.</p>
    </div>
    <div class="account-shell">
        <div class="module">
            <h3>Current access owner</h3>
            <div class="account-body">
                <p>The active profile for this session is <strong><asp:Literal ID="UserNameLiteral" runat="server" /></strong>.</p>
                <p class="form-note">This stub marks the legacy seam where password resets, MFA, and help-desk prompts will land in a future phase.</p>
                <asp:Button ID="BackToAccountButton" runat="server" Text="Back to account center" CssClass="secondary-button" OnClick="BackToAccountButton_Click" />
            </div>
        </div>
        <div class="module">
            <h3>Next retrofit targets</h3>
            <div class="account-body">
                <ul class="account-list">
                    <li>Replace emailed reset worksheets with token-based password reset flows.</li>
                    <li>Add help-desk managed lockout review and account recovery screens.</li>
                    <li>Surface security question retirement guidance once the wider portal retrofit is scheduled.</li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
