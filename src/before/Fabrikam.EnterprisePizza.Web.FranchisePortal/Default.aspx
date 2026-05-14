<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Web.FranchisePortal._Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="FreeTextBox" Namespace="FreeTextBoxControls" TagPrefix="ftb" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Franchise Bulletin Center</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <ajaxToolkit:Accordion ID="BulletinAccordion" runat="server" HeaderCssClass="accordionHeader" ContentCssClass="accordionBody">
            <Panes>
                <ajaxToolkit:AccordionPane runat="server">
                    <Header>Weekly Bulletin</Header>
                    <Content>
                        <asp:Label ID="BulletinTitle" runat="server" Text="Extreme Value Tuesday Field Notes" />
                        <br />
                        <ftb:FreeTextBox ID="BulletinEditor" runat="server" Height="160px" Width="620px" />
                    </Content>
                </ajaxToolkit:AccordionPane>
            </Panes>
        </ajaxToolkit:Accordion>
    </form>
</body>
</html>
