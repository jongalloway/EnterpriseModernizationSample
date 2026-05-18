using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IDimPartnerRepository
    {
        ReportingWarehouseDimensionsDataSet GetPartners(string partnerTier, bool includeInactive);
    }
}
