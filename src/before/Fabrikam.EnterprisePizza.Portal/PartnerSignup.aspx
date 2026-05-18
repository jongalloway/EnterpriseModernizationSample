<%@ Page Title="Partner Signup" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="PartnerSignup.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.PartnerSignup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeadPartnerSignup" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyPartnerSignup" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Partner Signup Intake</h2>
        <p>Capture new franchise and channel partner requests before the regional approval binder goes upstairs.</p>
    </div>

    <div class="module form-module">
        <h3>New partner application</h3>
        <div class="module-body">
            <asp:ValidationSummary ID="SignupValidationSummary" runat="server" CssClass="validation-summary" HeaderText="Please correct the highlighted fields:" />
            <asp:Panel ID="SuccessPanel" runat="server" CssClass="message-panel success-panel" Visible="false">
                <asp:Label ID="StatusMessageLabel" runat="server" />
            </asp:Panel>

            <asp:FormView ID="SignupFormView" runat="server" DataSourceID="PartnerApplicationSource" DefaultMode="Insert" OnItemInserted="SignupFormView_ItemInserted">
                <InsertItemTemplate>
                    <table class="entry-table">
                        <tr>
                            <th>Company name</th>
                            <td>
                                <asp:TextBox ID="CompanyNameTextBox" runat="server" Text='<%# Bind("CompanyName") %>' Width="280" />
                                <asp:RequiredFieldValidator ID="CompanyNameRequired" runat="server" ControlToValidate="CompanyNameTextBox" ErrorMessage="Company name is required." Display="Dynamic" CssClass="field-error" />
                            </td>
                            <th>Franchise region</th>
                            <td>
                                <asp:DropDownList ID="FranchiseRegionDropDown" runat="server" SelectedValue='<%# Bind("FranchiseRegion") %>'>
                                    <asp:ListItem Text="Puget Sound" Value="Puget Sound" />
                                    <asp:ListItem Text="North Metro" Value="North Metro" />
                                    <asp:ListItem Text="South Sound" Value="South Sound" />
                                    <asp:ListItem Text="Campus Corridor" Value="Campus Corridor" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>Business type</th>
                            <td>
                                <asp:DropDownList ID="BusinessTypeDropDown" runat="server" SelectedValue='<%# Bind("BusinessType") %>'>
                                    <asp:ListItem Text="Franchise group" Value="Franchise group" />
                                    <asp:ListItem Text="University channel" Value="University channel" />
                                    <asp:ListItem Text="Food court operator" Value="Food court operator" />
                                    <asp:ListItem Text="Corporate dining" Value="Corporate dining" />
                                </asp:DropDownList>
                            </td>
                            <th>Referral channel</th>
                            <td>
                                <asp:DropDownList ID="ReferralChannelDropDown" runat="server" SelectedValue='<%# Bind("ReferralChannel") %>'>
                                    <asp:ListItem Text="Broker referral" Value="Broker referral" />
                                    <asp:ListItem Text="Regional rep" Value="Regional rep" />
                                    <asp:ListItem Text="Partner expansion" Value="Partner expansion" />
                                    <asp:ListItem Text="Conference lead" Value="Conference lead" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>Primary contact</th>
                            <td>
                                <asp:TextBox ID="ContactNameTextBox" runat="server" Text='<%# Bind("ContactName") %>' Width="220" />
                                <asp:RequiredFieldValidator ID="ContactNameRequired" runat="server" ControlToValidate="ContactNameTextBox" ErrorMessage="Primary contact is required." Display="Dynamic" CssClass="field-error" />
                            </td>
                            <th>Email address</th>
                            <td>
                                <asp:TextBox ID="ContactEmailTextBox" runat="server" Text='<%# Bind("ContactEmail") %>' Width="240" />
                                <asp:RequiredFieldValidator ID="ContactEmailRequired" runat="server" ControlToValidate="ContactEmailTextBox" ErrorMessage="Email address is required." Display="Dynamic" CssClass="field-error" />
                                <asp:RegularExpressionValidator ID="ContactEmailValidator" runat="server" ControlToValidate="ContactEmailTextBox" ErrorMessage="Enter a valid email address." Display="Dynamic" CssClass="field-error" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
                            </td>
                        </tr>
                        <tr>
                            <th>Phone</th>
                            <td>
                                <asp:TextBox ID="ContactPhoneTextBox" runat="server" Text='<%# Bind("ContactPhone") %>' Width="180" />
                            </td>
                            <th>Sponsoring partner</th>
                            <td>
                                <asp:TextBox ID="SponsorPartnerTextBox" runat="server" Text='<%# Bind("SponsorPartner") %>' Width="240" />
                                <ajaxToolkit:AutoCompleteExtender ID="SponsorPartnerAutoComplete" runat="server" TargetControlID="SponsorPartnerTextBox" ServiceMethod="SearchPartners" MinimumPrefixLength="1" CompletionSetCount="8" CompletionInterval="150" EnableCaching="true" />
                            </td>
                        </tr>
                        <tr>
                            <th>Target launch date</th>
                            <td>
                                <asp:TextBox ID="TargetLaunchDateTextBox" runat="server" Text='<%# Bind("TargetLaunchDate") %>' Width="120" />
                                <ajaxToolkit:CalendarExtender ID="TargetLaunchCalendar" runat="server" TargetControlID="TargetLaunchDateTextBox" Format="MM/dd/yyyy" />
                            </td>
                            <th>Notes</th>
                            <td rowspan="2">
                                <asp:TextBox ID="NotesTextBox" runat="server" Text='<%# Bind("Notes") %>' TextMode="MultiLine" Rows="5" Width="260" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">Attach binder notes for pricing exceptions, fax packet follow-up, or training needs.</th>
                        </tr>
                    </table>

                    <div class="form-actions">
                        <asp:Button ID="SubmitApplicationButton" runat="server" CommandName="Insert" Text="Log Application" CssClass="portal-button" />
                        <asp:Button ID="ResetApplicationButton" runat="server" CommandName="New" Text="Reset Form" CausesValidation="false" CssClass="portal-button secondary-button" />
                    </div>
                </InsertItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <asp:ObjectDataSource ID="PartnerApplicationSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="CreateApplicationDraft" InsertMethod="SubmitPartnerApplication" />
</asp:Content>
