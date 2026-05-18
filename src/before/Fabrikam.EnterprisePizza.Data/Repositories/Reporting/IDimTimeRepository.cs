using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;

namespace Fabrikam.EnterprisePizza.Data.Repositories.Reporting
{
    public interface IDimTimeRepository
    {
        ReportingWarehouseDimensionsDataSet GetCalendarRange(DateTime startDate, DateTime endDate);
    }
}
