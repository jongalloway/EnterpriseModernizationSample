<%@ Page Title="Store Operations Dashboard" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Fabrikam.EnterprisePizza.StoreOps.Portal._Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeadContentBlock" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="MainContentBlock" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="PortalScriptManager" runat="server" />
    <asp:HiddenField ID="SelectedStoreNumber" runat="server" ClientIDMode="Static" />

    <div class="dashboardHero">
        <div class="heroCopy">
            <h1>Dispatch, labor, and zone follow-up in one supervisor shell</h1>
            <p>
                This portal keeps late-shift dispatch calls, labor watch items, and handwritten zone reminders
                together in the same intranet surface the field team has been nursing along for years.
            </p>
        </div>
        <div class="heroBadge">
            <span class="badgeLabel">Tonight's focus</span>
            <span class="badgeValue">Driver coverage + labor holds</span>
        </div>
    </div>

    <div class="statusRibbon">
        <div class="statusCard">
            <span class="statusLabel">Dispatch board</span>
            <span class="statusValue">Live snapshot from StoreOps coordinator</span>
        </div>
        <div class="statusCard statusWarning">
            <span class="statusLabel">Labor watch</span>
            <span class="statusValue">Keep overtime approvals under 4.0 hours</span>
        </div>
        <div class="statusCard">
            <span class="statusLabel">Zone bulletin</span>
            <span class="statusValue">Northwest corridor requires manual note checks</span>
        </div>
    </div>

    <div class="dashboardLayout">
        <div class="dashboardMain">
            <ajaxToolkit:TabContainer ID="OperationsTabs" runat="server" ActiveTabIndex="0" CssClass="operationsTabs">
                <ajaxToolkit:TabPanel ID="DispatchTab" runat="server" HeaderText="Dispatch Queue">
                    <ContentTemplate>
                        <div id="dispatchBoard" class="legacyPanel">
                            <asp:UpdatePanel ID="DispatchUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="toolbar">
                                        <label for="<%= StoreSelector.ClientID %>">Store</label>
                                        <asp:DropDownList ID="StoreSelector" runat="server" CssClass="legacySelect" />
                                        <asp:Button ID="RefreshBoardButton" runat="server" Text="Refresh Board" CssClass="legacyButton" OnClick="RefreshBoardButton_Click" />
                                        <span class="toolbarNote">Dinner rush board refreshes through the same sample coordinator used by desktop dispatch.</span>
                                    </div>

                                    <asp:GridView ID="ActiveRoutesGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" GridLines="None">
                                        <Columns>
                                            <asp:BoundField HeaderText="Ticket" DataField="TicketId" />
                                            <asp:BoundField HeaderText="Driver" DataField="DriverCode" />
                                            <asp:BoundField HeaderText="Route Zone" DataField="RouteZone" />
                                            <asp:BoundField HeaderText="Status" DataField="DispatchState" />
                                            <asp:BoundField HeaderText="Minutes Open" DataField="MinutesOpen" />
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="LaborTab" runat="server" HeaderText="Labor Watch">
                    <ContentTemplate>
                        <div id="laborWatch" class="legacyPanel">
                            <asp:GridView ID="LaborWatchGrid" runat="server" AutoGenerateColumns="False" CssClass="legacyGrid" GridLines="None">
                                <Columns>
                                    <asp:BoundField HeaderText="Store" DataField="StoreNumber" />
                                    <asp:BoundField HeaderText="Team" DataField="TeamName" />
                                    <asp:BoundField HeaderText="Concern" DataField="Concern" />
                                    <asp:BoundField HeaderText="Action" DataField="ActionRequired" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="ZoneNotesTab" runat="server" HeaderText="Zone Notes">
                    <ContentTemplate>
                        <div id="zoneNotes" class="legacyPanel">
                            <asp:BulletedList ID="ZoneBulletinList" runat="server" CssClass="zoneBulletins" />
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>

        <div class="dashboardSidebar">
            <div class="sidebarPanel">
                <h2>Supervisor quick actions</h2>
                <p>Use these shortcuts before the dinner window gets noisy.</p>
                <asp:LinkButton ID="TerminalReminderLink" runat="server" CssClass="legacyButtonLink">Show terminal reminders</asp:LinkButton>
                <div class="sidebarLinkGroup">
                    <a href="OrderLookup.aspx" class="legacyButtonLink">Open order lookup</a>
                    <a href="OrderHistory.aspx" class="legacyButtonLink">Review order history</a>
                </div>
            </div>

            <div class="sidebarPanel sidebarPanelMuted">
                <h2>Why this page feels old</h2>
                <p>
                    Supervisors still bounce between Web Forms tabs, a modal reminder panel, and the desktop dispatch board.
                    It is clunky, but it tells the right before-state story.
                </p>
            </div>
        </div>
    </div>

    <asp:Panel ID="TerminalReminderPanel" runat="server" CssClass="modalPanel" Style="display: none;">
        <div class="modalHeader">Supervisor terminal reminders</div>
        <ul>
            <li>Refresh dispatch after a driver swap or zone handoff.</li>
            <li>Print labor watch before approving overtime exceptions.</li>
            <li>Keep the dispatch desktop board visible on the second monitor during rush.</li>
        </ul>
        <asp:Button ID="DismissTerminalReminderButton" runat="server" Text="Close" CssClass="legacyButton" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="TerminalReminderPopup" runat="server" TargetControlID="TerminalReminderLink" PopupControlID="TerminalReminderPanel" CancelControlID="DismissTerminalReminderButton" BackgroundCssClass="modalBackdrop" />
</asp:Content>
