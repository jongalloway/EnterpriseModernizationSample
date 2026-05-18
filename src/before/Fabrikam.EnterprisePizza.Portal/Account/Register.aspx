<%@ Page Title="Register Partner Account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.Account.Register" %>
<asp:Content ID="RegisterMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Register a Partner Account</h2>
        <p>Create a new legacy portal profile for a district lead, franchise operator, or catering support specialist.</p>
    </div>
    <div class="account-shell">
        <div class="module auth-card">
            <h3>New account request</h3>
            <div class="auth-body">
                <asp:ValidationSummary ID="RegisterValidationSummary" runat="server" CssClass="validation-summary" />
                <asp:Label ID="FailureMessageLabel" runat="server" CssClass="error-banner" Visible="false" />
                <div class="auth-form-row">
                    <span>Corporate email</span>
                    <asp:TextBox ID="EmailTextBox" runat="server" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Enter a corporate email address." Display="Dynamic" CssClass="field-error" />
                </div>
                <div class="auth-form-row">
                    <span>User name</span>
                    <asp:TextBox ID="UserNameTextBox" runat="server" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="UserNameRequiredValidator" runat="server" ControlToValidate="UserNameTextBox" ErrorMessage="Enter a user name." Display="Dynamic" CssClass="field-error" />
                </div>
                <div class="auth-form-row">
                    <span>Password</span>
                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Enter a password." Display="Dynamic" CssClass="field-error" />
                </div>
                <div class="auth-form-row">
                    <span>Confirm password</span>
                    <asp:TextBox ID="ConfirmPasswordTextBox" runat="server" TextMode="Password" CssClass="text-input" />
                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequiredValidator" runat="server" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="Confirm the password." Display="Dynamic" CssClass="field-error" />
                    <asp:CompareValidator ID="PasswordCompareValidator" runat="server" ControlToCompare="PasswordTextBox" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="The passwords must match." Display="Dynamic" CssClass="field-error" />
                </div>
                <asp:Button ID="RegisterButton" runat="server" Text="Register Account" CssClass="primary-button" OnClick="RegisterButton_Click" />
                <div class="form-note">
                    Passwords must be at least six characters and include at least one number. Existing reps can <a href="Login.aspx">sign in here</a>.
                </div>
            </div>
        </div>
        <div class="module">
            <h3>Legacy provisioning workflow</h3>
            <div class="account-body">
                <ul class="account-list">
                    <li>Use a valid Fabrikam or partner-company email so the old account export matches district contact sheets.</li>
                    <li>Identity records stay in a dedicated portal membership store, separate from the storefront and StoreOps systems.</li>
                    <li>Additional role assignment and password reset workflows are still handled by the help desk after account creation.</li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
