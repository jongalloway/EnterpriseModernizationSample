using System;
namespace Fabrikam.EnterprisePizza.Portal
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            QuickLaunchRepeater.DataSource = new[]
            {
                new PortalLink("Corporate catering queue", "/CustomerHub/CorporateAccounts.aspx", "Review preferred partner accounts and re-key faxed orders."),
                new PortalLink("Franchise bulletin center", "/FranchisePortal/Default.aspx", "Publish campaign art, compliance notices, and district talking points."),
                new PortalLink("Online storefront", "/Storefront/", "Check customer-facing offers before the Tuesday coupon blast.")
            };
            QuickLaunchRepeater.DataBind();

            HeadlineRepeater.DataSource = new[]
            {
                new PortalHeadline("Coupon kit posted", "PROMO", "Extreme Value Tuesday artwork is staged for all 42 stores. Print packets before lunch for the counter teams."),
                new PortalHeadline("Driver maps refreshed", "OPS", "Updated delivery-zone notes were uploaded after the overnight MapQuest route export completed."),
                new PortalHeadline("Catering follow-up", "SALES", "Eight corporate account renewals still need signed menu pricing sheets from regional reps.")
            };
            HeadlineRepeater.DataBind();

            ChecklistBullets.DataSource = new[]
            {
                "Confirm overnight POS sync exceptions were cleared before 8:30 AM.",
                "Email the district newsletter signup reminder to store coordinators.",
                "Upload revised compliance PDF packets for any stores missing window-cling artwork."
            };
            ChecklistBullets.DataBind();

            PromoRepeater.DataSource = new[]
            {
                new PromoStatus("Extreme Value Tuesday", "Tue-Fri", "Ready for print"),
                new PromoStatus("Family Feast Fax Offer", "This week", "Needs district approval"),
                new PromoStatus("Partner Lunch Bundle", "Month end", "Awaiting coupon code refresh")
            };
            PromoRepeater.DataBind();
        }

        private sealed class PortalLink
        {
            public PortalLink(string title, string url, string description)
            {
                Title = title;
                Url = url;
                Description = description;
            }

            public string Title { get; private set; }

            public string Url { get; private set; }

            public string Description { get; private set; }
        }

        private sealed class PortalHeadline
        {
            public PortalHeadline(string title, string badge, string body)
            {
                Title = title;
                Badge = badge;
                Body = body;
            }

            public string Title { get; private set; }

            public string Badge { get; private set; }

            public string Body { get; private set; }
        }

        private sealed class PromoStatus
        {
            public PromoStatus(string name, string window, string status)
            {
                Name = name;
                Window = window;
                Status = status;
            }

            public string Name { get; private set; }

            public string Window { get; private set; }

            public string Status { get; private set; }
        }
    }
}
