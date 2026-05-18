using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IDimDriverRepository
    {
        ReportingWarehouseDimensionsDataSet GetDrivers(string storeNumber, bool includeInactive);
    }
}
