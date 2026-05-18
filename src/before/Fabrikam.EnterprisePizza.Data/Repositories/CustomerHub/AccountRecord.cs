namespace Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub
{
    public class AccountRecord
    {
        public int CorporateAccountId { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public int? PreferredPartnerAccountId { get; set; }

        public string BillingFrequency { get; set; }

        public bool ActiveFlag { get; set; }

        public string AccountTier { get; set; }

        public string ExternalAccountCode { get; set; }

        public string AccountManagerName { get; set; }

        public string StatusCode { get; set; }
    }
}
