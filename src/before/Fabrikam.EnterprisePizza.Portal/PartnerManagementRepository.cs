using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Portal
{
    public class PartnerManagementRepository
    {
        private static readonly object SyncRoot = new object();
        private static readonly DateTime SnapshotDate = new DateTime(2026, 5, 18);
        private static readonly List<PartnerApplicationRecord> Applications = new List<PartnerApplicationRecord>
        {
            new PartnerApplicationRecord
            {
                ApplicationId = 4101,
                CompanyName = "North Sound Hospitality Group",
                FranchiseRegion = "Puget Sound",
                BusinessType = "Franchise group",
                ContactName = "Carla Ruiz",
                ContactEmail = "carla.ruiz@northsoundhg.example.com",
                ContactPhone = "(206) 555-0184",
                SponsorPartner = "Adventure Works Corporate Dining",
                TargetLaunchDate = new DateTime(2026, 6, 8),
                ReferralChannel = "Broker referral",
                Notes = "Rolling three airport-adjacent franchise counters into the Tuesday value bundle.",
                Status = "Pending intake",
                SubmittedOn = new DateTime(2026, 5, 14)
            },
            new PartnerApplicationRecord
            {
                ApplicationId = 4102,
                CompanyName = "Cascade Campus Eats",
                FranchiseRegion = "South Sound",
                BusinessType = "University channel",
                ContactName = "Miles Dorsey",
                ContactEmail = "miles.dorsey@cascadecampuseats.example.com",
                ContactPhone = "(253) 555-0119",
                SponsorPartner = "Woodgrove Business Catering",
                TargetLaunchDate = new DateTime(2026, 6, 15),
                ReferralChannel = "Regional rep",
                Notes = "Needs menu board artwork and late-night delivery rider review.",
                Status = "Awaiting packet",
                SubmittedOn = new DateTime(2026, 5, 16)
            },
            new PartnerApplicationRecord
            {
                ApplicationId = 4103,
                CompanyName = "Blue Yonder Food Courts",
                FranchiseRegion = "North Metro",
                BusinessType = "Food court operator",
                ContactName = "Amelia Hart",
                ContactEmail = "amelia.hart@blueyondercourts.example.com",
                ContactPhone = "(425) 555-0160",
                SponsorPartner = "Litware Office Services",
                TargetLaunchDate = new DateTime(2026, 6, 21),
                ReferralChannel = "Partner expansion",
                Notes = "Contract packet needs revised beverage syrup minimums before signoff.",
                Status = "Pending intake",
                SubmittedOn = new DateTime(2026, 5, 17)
            }
        };

        private static readonly List<PartnerContractRecord> Contracts = new List<PartnerContractRecord>
        {
            new PartnerContractRecord { ContractId = 20041, PartnerName = "Adventure Works Corporate Dining", AgreementType = "Master catering referral", EffectiveDate = new DateTime(2025, 7, 1), RenewalDate = new DateTime(2026, 6, 30), Status = "Renewal review", AccountManager = "T. Jenkins", RateCard = "$3.25 / order" },
            new PartnerContractRecord { ContractId = 20044, PartnerName = "Contoso Office Parks", AgreementType = "Employee lunch subsidy", EffectiveDate = new DateTime(2026, 1, 15), RenewalDate = new DateTime(2026, 12, 31), Status = "Executed", AccountManager = "L. Monroe", RateCard = "7% rebate" },
            new PartnerContractRecord { ContractId = 20048, PartnerName = "Woodgrove Business Catering", AgreementType = "Regional franchise support", EffectiveDate = new DateTime(2025, 10, 1), RenewalDate = new DateTime(2026, 5, 29), Status = "Pending signature", AccountManager = "J. Alvarez", RateCard = "$2,800 monthly" },
            new PartnerContractRecord { ContractId = 20053, PartnerName = "Northwind Campus Rewards", AgreementType = "Referral channel addendum", EffectiveDate = new DateTime(2026, 3, 20), RenewalDate = new DateTime(2027, 3, 19), Status = "Needs legal review", AccountManager = "P. Saunders", RateCard = "9% commission" },
            new PartnerContractRecord { ContractId = 20057, PartnerName = "Blue Yonder Food Courts", AgreementType = "Franchise onboarding packet", EffectiveDate = new DateTime(2026, 4, 10), RenewalDate = new DateTime(2026, 6, 12), Status = "Pending signature", AccountManager = "R. Coleman", RateCard = "$1,750 setup" },
            new PartnerContractRecord { ContractId = 20062, PartnerName = "Litware Office Services", AgreementType = "Referral pilot extension", EffectiveDate = new DateTime(2026, 2, 1), RenewalDate = new DateTime(2026, 7, 31), Status = "Executed", AccountManager = "T. Jenkins", RateCard = "6% commission" }
        };

        private static readonly List<ReferralRecord> Referrals = new List<ReferralRecord>
        {
            new ReferralRecord { ReferralId = 87011, PartnerName = "Adventure Works Corporate Dining", ReferralChannel = "Broker referral", CustomerName = "A. Datum Capital", AttributedOn = new DateTime(2026, 5, 5), OrdersBooked = 9, CommissionEarned = 486.00m, Status = "Qualified" },
            new ReferralRecord { ReferralId = 87016, PartnerName = "Woodgrove Business Catering", ReferralChannel = "Regional rep", CustomerName = "Consolidated Messenger", AttributedOn = new DateTime(2026, 5, 7), OrdersBooked = 6, CommissionEarned = 318.50m, Status = "Qualified" },
            new ReferralRecord { ReferralId = 87018, PartnerName = "Contoso Office Parks", ReferralChannel = "Lobby promo kiosk", CustomerName = "Graphic Design Institute", AttributedOn = new DateTime(2026, 5, 9), OrdersBooked = 4, CommissionEarned = 124.00m, Status = "Paid" },
            new ReferralRecord { ReferralId = 87022, PartnerName = "Blue Yonder Food Courts", ReferralChannel = "Partner expansion", CustomerName = "Fabrikam Travel Desk", AttributedOn = new DateTime(2026, 5, 11), OrdersBooked = 3, CommissionEarned = 96.00m, Status = "Pending attribution" },
            new ReferralRecord { ReferralId = 87027, PartnerName = "Northwind Campus Rewards", ReferralChannel = "Student orientation booth", CustomerName = "Northwind Student Union", AttributedOn = new DateTime(2026, 5, 13), OrdersBooked = 5, CommissionEarned = 201.25m, Status = "Qualified" },
            new ReferralRecord { ReferralId = 87029, PartnerName = "Litware Office Services", ReferralChannel = "Office concierge bulletin", CustomerName = "Litware Support Center", AttributedOn = new DateTime(2026, 5, 16), OrdersBooked = 7, CommissionEarned = 287.75m, Status = "Paid" }
        };

        public PartnerApplicationDraft CreateApplicationDraft()
        {
            return new PartnerApplicationDraft
            {
                BusinessType = "Franchise group",
                FranchiseRegion = "Puget Sound",
                ReferralChannel = "Broker referral",
                TargetLaunchDate = SnapshotDate.AddDays(21).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            };
        }

        public void SubmitPartnerApplication(string companyName, string franchiseRegion, string businessType, string contactName, string contactEmail, string contactPhone, string sponsorPartner, string targetLaunchDate, string referralChannel, string notes)
        {
            DateTime parsedLaunchDate;
            if (!DateTime.TryParse(targetLaunchDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedLaunchDate))
            {
                parsedLaunchDate = SnapshotDate.AddDays(21);
            }

            lock (SyncRoot)
            {
                Applications.Insert(0, new PartnerApplicationRecord
                {
                    ApplicationId = Applications.Max(item => item.ApplicationId) + 1,
                    CompanyName = companyName,
                    FranchiseRegion = franchiseRegion,
                    BusinessType = businessType,
                    ContactName = contactName,
                    ContactEmail = contactEmail,
                    ContactPhone = contactPhone,
                    SponsorPartner = sponsorPartner,
                    TargetLaunchDate = parsedLaunchDate,
                    ReferralChannel = referralChannel,
                    Notes = notes,
                    Status = "Pending intake",
                    SubmittedOn = SnapshotDate
                });
            }
        }

        public IList<PartnerApplicationRecord> GetPendingApplications()
        {
            lock (SyncRoot)
            {
                return Applications
                    .OrderByDescending(item => item.SubmittedOn)
                    .ThenBy(item => item.CompanyName)
                    .Take(5)
                    .ToList();
            }
        }

        public IList<PartnerContractRecord> GetContracts(string partnerName, string status, string sortExpression)
        {
            lock (SyncRoot)
            {
                IEnumerable<PartnerContractRecord> query = Contracts;
                query = FilterContracts(query, partnerName, status);
                return ApplyContractSort(query, sortExpression).ToList();
            }
        }

        public void UpdateContract(int contractId, string status, string accountManager)
        {
            lock (SyncRoot)
            {
                var contract = Contracts.FirstOrDefault(item => item.ContractId == contractId);
                if (contract == null)
                {
                    return;
                }

                contract.Status = status;
                contract.AccountManager = accountManager;
            }
        }

        public ReferralSummary GetReferralSummary(string partnerName, string status)
        {
            var referrals = FilterReferrals(Referrals, partnerName, status).ToList();
            var totalCount = referrals.Count;
            var commissionTotal = referrals.Sum(item => item.CommissionEarned);
            var qualifiedCount = referrals.Count(item => string.Equals(item.Status, "Qualified", StringComparison.OrdinalIgnoreCase));

            return new ReferralSummary
            {
                TotalReferrals = totalCount,
                QualifiedReferrals = qualifiedCount,
                CommissionEarned = commissionTotal,
                AverageCommission = totalCount == 0 ? 0m : commissionTotal / totalCount
            };
        }

        public IList<ReferralRecord> GetReferrals(string partnerName, string status, string sortExpression)
        {
            lock (SyncRoot)
            {
                IEnumerable<ReferralRecord> query = Referrals;
                query = FilterReferrals(query, partnerName, status);
                return ApplyReferralSort(query, sortExpression).ToList();
            }
        }

        public PartnerDashboardSummary GetDashboardSummary()
        {
            lock (SyncRoot)
            {
                return new PartnerDashboardSummary
                {
                    ActivePartners = Contracts.Select(item => item.PartnerName).Distinct().Count(),
                    PendingApplications = Applications.Count(item => !string.Equals(item.Status, "Approved", StringComparison.OrdinalIgnoreCase)),
                    ContractsAwaitingAction = Contracts.Count(item => !string.Equals(item.Status, "Executed", StringComparison.OrdinalIgnoreCase)),
                    MonthlyReferralCommissions = Referrals.Sum(item => item.CommissionEarned),
                    FranchiseLaunches = Applications.Count(item => item.TargetLaunchDate >= SnapshotDate && item.TargetLaunchDate <= SnapshotDate.AddDays(45))
                };
            }
        }

        public IList<ContractWatchItem> GetContractWatchList()
        {
            lock (SyncRoot)
            {
                return Contracts
                    .Where(item => item.RenewalDate <= SnapshotDate.AddDays(60) || !string.Equals(item.Status, "Executed", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(item => item.RenewalDate)
                    .Select(item => new ContractWatchItem
                    {
                        PartnerName = item.PartnerName,
                        AgreementType = item.AgreementType,
                        RenewalDate = item.RenewalDate,
                        Status = item.Status
                    })
                    .Take(5)
                    .ToList();
            }
        }

        public IList<ReferralLeaderItem> GetReferralLeaders()
        {
            lock (SyncRoot)
            {
                return Referrals
                    .GroupBy(item => item.PartnerName)
                    .Select(group => new ReferralLeaderItem
                    {
                        PartnerName = group.Key,
                        QualifiedReferrals = group.Count(item => string.Equals(item.Status, "Qualified", StringComparison.OrdinalIgnoreCase) || string.Equals(item.Status, "Paid", StringComparison.OrdinalIgnoreCase)),
                        CommissionEarned = group.Sum(item => item.CommissionEarned)
                    })
                    .OrderByDescending(item => item.CommissionEarned)
                    .ThenBy(item => item.PartnerName)
                    .Take(5)
                    .ToList();
            }
        }

        public string[] SearchPartnerNames(string prefixText, int count)
        {
            var normalizedPrefix = prefixText ?? string.Empty;

            lock (SyncRoot)
            {
                return Contracts
                    .Select(item => item.PartnerName)
                    .Concat(Applications.Select(item => item.CompanyName))
                    .Distinct()
                    .Where(item => item.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(item => item)
                    .Take(count <= 0 ? 8 : count)
                    .ToArray();
            }
        }

        private static IEnumerable<PartnerContractRecord> FilterContracts(IEnumerable<PartnerContractRecord> items, string partnerName, string status)
        {
            var filteredItems = items;

            if (!string.IsNullOrWhiteSpace(partnerName))
            {
                filteredItems = filteredItems.Where(item => item.PartnerName.IndexOf(partnerName.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                filteredItems = filteredItems.Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return filteredItems;
        }

        private static IEnumerable<ReferralRecord> FilterReferrals(IEnumerable<ReferralRecord> items, string partnerName, string status)
        {
            var filteredItems = items;

            if (!string.IsNullOrWhiteSpace(partnerName))
            {
                filteredItems = filteredItems.Where(item => item.PartnerName.IndexOf(partnerName.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                filteredItems = filteredItems.Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return filteredItems;
        }

        private static IEnumerable<PartnerContractRecord> ApplyContractSort(IEnumerable<PartnerContractRecord> items, string sortExpression)
        {
            switch ((sortExpression ?? string.Empty).Trim())
            {
                case "PartnerName DESC":
                    return items.OrderByDescending(item => item.PartnerName);
                case "PartnerName":
                    return items.OrderBy(item => item.PartnerName);
                case "AgreementType DESC":
                    return items.OrderByDescending(item => item.AgreementType);
                case "AgreementType":
                    return items.OrderBy(item => item.AgreementType);
                case "RenewalDate DESC":
                    return items.OrderByDescending(item => item.RenewalDate);
                case "RenewalDate":
                    return items.OrderBy(item => item.RenewalDate);
                case "Status DESC":
                    return items.OrderByDescending(item => item.Status);
                case "Status":
                    return items.OrderBy(item => item.Status);
                default:
                    return items.OrderBy(item => item.RenewalDate).ThenBy(item => item.PartnerName);
            }
        }

        private static IEnumerable<ReferralRecord> ApplyReferralSort(IEnumerable<ReferralRecord> items, string sortExpression)
        {
            switch ((sortExpression ?? string.Empty).Trim())
            {
                case "PartnerName DESC":
                    return items.OrderByDescending(item => item.PartnerName);
                case "PartnerName":
                    return items.OrderBy(item => item.PartnerName);
                case "AttributedOn DESC":
                    return items.OrderByDescending(item => item.AttributedOn);
                case "AttributedOn":
                    return items.OrderBy(item => item.AttributedOn);
                case "CommissionEarned DESC":
                    return items.OrderByDescending(item => item.CommissionEarned);
                case "CommissionEarned":
                    return items.OrderBy(item => item.CommissionEarned);
                case "Status DESC":
                    return items.OrderByDescending(item => item.Status);
                case "Status":
                    return items.OrderBy(item => item.Status);
                default:
                    return items.OrderByDescending(item => item.AttributedOn).ThenBy(item => item.PartnerName);
            }
        }
    }

    public class PartnerApplicationDraft
    {
        public string CompanyName { get; set; }

        public string FranchiseRegion { get; set; }

        public string BusinessType { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string SponsorPartner { get; set; }

        public string TargetLaunchDate { get; set; }

        public string ReferralChannel { get; set; }

        public string Notes { get; set; }
    }

    public class PartnerApplicationRecord
    {
        public int ApplicationId { get; set; }

        public string CompanyName { get; set; }

        public string FranchiseRegion { get; set; }

        public string BusinessType { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string SponsorPartner { get; set; }

        public DateTime TargetLaunchDate { get; set; }

        public string ReferralChannel { get; set; }

        public string Notes { get; set; }

        public string Status { get; set; }

        public DateTime SubmittedOn { get; set; }
    }

    public class PartnerContractRecord
    {
        public int ContractId { get; set; }

        public string PartnerName { get; set; }

        public string AgreementType { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime RenewalDate { get; set; }

        public string Status { get; set; }

        public string AccountManager { get; set; }

        public string RateCard { get; set; }
    }

    public class ReferralRecord
    {
        public int ReferralId { get; set; }

        public string PartnerName { get; set; }

        public string ReferralChannel { get; set; }

        public string CustomerName { get; set; }

        public DateTime AttributedOn { get; set; }

        public int OrdersBooked { get; set; }

        public decimal CommissionEarned { get; set; }

        public string Status { get; set; }
    }

    public class ReferralSummary
    {
        public int TotalReferrals { get; set; }

        public int QualifiedReferrals { get; set; }

        public decimal CommissionEarned { get; set; }

        public decimal AverageCommission { get; set; }
    }

    public class PartnerDashboardSummary
    {
        public int ActivePartners { get; set; }

        public int PendingApplications { get; set; }

        public int ContractsAwaitingAction { get; set; }

        public decimal MonthlyReferralCommissions { get; set; }

        public int FranchiseLaunches { get; set; }
    }

    public class ContractWatchItem
    {
        public string PartnerName { get; set; }

        public string AgreementType { get; set; }

        public DateTime RenewalDate { get; set; }

        public string Status { get; set; }
    }

    public class ReferralLeaderItem
    {
        public string PartnerName { get; set; }

        public int QualifiedReferrals { get; set; }

        public decimal CommissionEarned { get; set; }
    }
}
