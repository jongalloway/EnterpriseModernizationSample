<%@ Page Title="Partner Portal Home" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Fabrikam.EnterprisePizza.Portal._Default" %>
<asp:Content ID="HeadDefault" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyDefault" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h2>Monday Morning Partner Snapshot</h2>
        <p>One screen for the weekly promo sheet, district notes, and support queue backlog.</p>
    </div>

    <div class="column-wrap">
        <div class="module feature-module">
            <h3>Quick Launch</h3>
            <asp:Repeater ID="QuickLaunchRepeater" runat="server">
                <ItemTemplate>
                    <div class="quick-link">
                        <a href="<%# EncodeHref(Eval("Url")) %>"><%# Encode(Eval("Title")) %></a>
                        <span><%# Encode(Eval("Description")) %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="module">
            <h3>Operations Headlines</h3>
            <asp:Repeater ID="HeadlineRepeater" runat="server">
                <ItemTemplate>
                    <div class="headline-card">
                        <div class="headline-badge"><%# Encode(Eval("Badge")) %></div>
                        <strong><%# Encode(Eval("Title")) %></strong>
                        <p><%# Encode(Eval("Body")) %></p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <div class="column-wrap">
        <div class="module">
            <h3>Support Center Checklist</h3>
            <asp:BulletedList ID="ChecklistBullets" runat="server" CssClass="checklist" />
        </div>

        <div class="module">
            <h3>Promo Watch</h3>
            <asp:Repeater ID="PromoRepeater" runat="server">
                <ItemTemplate>
                    <div class="promo-row">
                        <span class="promo-name"><%# Encode(Eval("Name")) %></span>
                        <span class="promo-window"><%# Encode(Eval("Window")) %></span>
                        <span class="promo-status"><%# Encode(Eval("Status")) %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="newsletter-box">
                <strong>Partner Email Club</strong>
                Sign up district assistants for promo reminders, coupon PDFs, and nightly exception callouts.
            </div>
        </div>
    </div>
</asp:Content>
