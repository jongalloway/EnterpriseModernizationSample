<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CorporateAccounts.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Web.CustomerHub.CorporateAccounts" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Corporate Catering Accounts</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <ajaxToolkit:TabContainer ID="AccountTabs" runat="server">
            <ajaxToolkit:TabPanel runat="server" HeaderText="Preferred Partners">
                <ContentTemplate>
                    <asp:GridView ID="PartnerGrid" runat="server" AutoGenerateColumns="true" />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </form>
</body>
</html>
