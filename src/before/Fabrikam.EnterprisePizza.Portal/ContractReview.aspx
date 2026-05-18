<%@ Page Title="Contract Review" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ContractReview.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal.ContractReview" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeadContractReview" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContractReview" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Partner Contract Review</h2>
        <p>Use the weekly binder list to search agreements, sort upcoming renewals, and keep signature status current.</p>
    </div>

    <div class="module">
        <h3>Filter contracts</h3>
        <div class="module-body filter-bar">
            <div class="filter-field">
                <label for="PartnerSearchTextBox">Partner name</label>
                <asp:TextBox ID="PartnerSearchTextBox" runat="server" Width="220" />
                <ajaxToolkit:AutoCompleteExtender ID="ContractPartnerAutoComplete" runat="server" TargetControlID="PartnerSearchTextBox" ServiceMethod="SearchPartners" MinimumPrefixLength="1" CompletionSetCount="8" CompletionInterval="150" EnableCaching="true" />
            </div>
            <div class="filter-field">
                <label for="ContractStatusFilter">Status</label>
                <asp:DropDownList ID="ContractStatusFilter" runat="server">
                    <asp:ListItem Text="All statuses" Value="All" />
                    <asp:ListItem Text="Pending signature" Value="Pending signature" />
                    <asp:ListItem Text="Renewal review" Value="Renewal review" />
                    <asp:ListItem Text="Needs legal review" Value="Needs legal review" />
                    <asp:ListItem Text="Executed" Value="Executed" />
                </asp:DropDownList>
            </div>
            <div class="filter-actions">
                <asp:Button ID="ApplyContractFiltersButton" runat="server" Text="Apply Filters" CssClass="portal-button" />
            </div>
        </div>
    </div>

    <div class="module">
        <h3>Contract queue</h3>
        <asp:GridView ID="ContractGrid" runat="server" CssClass="portal-grid" DataSourceID="ContractDataSource" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true" PageSize="5" DataKeyNames="ContractId" AutoGenerateEditButton="true" EmptyDataText="No partner contracts matched the current filters.">
            <Columns>
                <asp:BoundField DataField="ContractId" HeaderText="Contract #" ReadOnly="true" SortExpression="ContractId" />
                <asp:BoundField DataField="PartnerName" HeaderText="Partner" ReadOnly="true" SortExpression="PartnerName" />
                <asp:BoundField DataField="AgreementType" HeaderText="Agreement" ReadOnly="true" SortExpression="AgreementType" />
                <asp:BoundField DataField="EffectiveDate" HeaderText="Effective" ReadOnly="true" SortExpression="EffectiveDate" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="RenewalDate" HeaderText="Renewal" ReadOnly="true" SortExpression="RenewalDate" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate>
                        <%# Eval("Status") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="StatusDropDown" runat="server" SelectedValue='<%# Bind("Status") %>'>
                            <asp:ListItem Text="Pending signature" Value="Pending signature" />
                            <asp:ListItem Text="Renewal review" Value="Renewal review" />
                            <asp:ListItem Text="Needs legal review" Value="Needs legal review" />
                            <asp:ListItem Text="Executed" Value="Executed" />
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AccountManager" HeaderText="Account manager" SortExpression="AccountManager" />
                <asp:BoundField DataField="RateCard" HeaderText="Rate card" ReadOnly="true" SortExpression="RateCard" />
            </Columns>
        </asp:GridView>
    </div>

    <asp:ObjectDataSource ID="ContractDataSource" runat="server" TypeName="Fabrikam.EnterprisePizza.Portal.PartnerManagementRepository" SelectMethod="GetContracts" UpdateMethod="UpdateContract" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="PartnerSearchTextBox" Name="partnerName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ContractStatusFilter" Name="status" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
