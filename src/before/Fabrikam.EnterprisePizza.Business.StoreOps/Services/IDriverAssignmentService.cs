using System.Collections.Generic;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public interface IDriverAssignmentService
    {
        IList<DriverAvailability> GetAvailableDrivers(string storeNumber, IList<DispatchTicket> tickets);

        IList<DriverAssignmentRecommendation> RecommendAssignments(string storeNumber, IList<DispatchTicket> tickets, IList<DriverAvailability> drivers);
    }
}
