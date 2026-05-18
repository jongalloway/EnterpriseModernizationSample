<%@ Page Title="Partner Sign In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.Account.Login" %>
<asp:Content ID="LoginMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Trusted Partner Sign In</h2>
        <p>Use your legacy partner portal credentials to review district rollups, corporate catering activity, and bulletin packets.</p>
    </div>
    <div class="account-shell">
        <div class="module auth-card">
            <h3>Legacy portal access</h3>
            <div class="auth-body">
                <asp:ValidationSummary ID="LoginValidationSummary" runat="server" CssClass="validation-summary" />
                <asp:Label ID="FailureMessageLabel" runat="server" CssClass="error-banner" Visible="false" />
                <div class="auth-form-row">
                    <span>User name or email</span>
                    <asp:TextBox ID="UserNameTextBox" runat="server" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="UserNameRequiredValidator" runat="server" ControlToValidate="UserNameTextBox" ErrorMessage="Enter your user name or email." Display="Dynamic" CssClass="field-error" />
                </div>
                <div class="auth-form-row">
                    <span>Password</span>
                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Enter your password." Display="Dynamic" CssClass="field-error" />
                </div>
                <div class="auth-form-row check-row">
                    <asp:CheckBox ID="RememberMeCheckBox" runat="server" Text="Keep me signed in on this office workstation" />
                </div>
                <asp:Button ID="SignInButton" runat="server" Text="Sign In" CssClass="primary-button" OnClick="SignInButton_Click" />
                <div class="form-note">
                    Need access for a new district rep or franchise operator? <a href="Register.aspx">Create a portal account</a> with your corporate email address.
                </div>
            </div>
        </div>
        <div class="module">
            <h3>Access notes</h3>
            <div class="account-body">
                <ul class="account-list">
                    <li>Partner accounts use classic ASP.NET Identity cookie authentication layered over the legacy Web Forms shell.</li>
                    <li>Regional managers should continue using shared floor kiosks only after verifying the previous user signed out.</li>
                    <li>Contact HQ support before 4:00 PM if your corporate email changes and the account needs to be re-linked.</li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
