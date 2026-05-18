using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IDimStoreRepository
    {
        ReportingWarehouseDimensionsDataSet GetStores(string regionName, bool includeInactive);
    }
}
