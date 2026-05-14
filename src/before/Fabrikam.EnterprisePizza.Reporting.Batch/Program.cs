using System;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;

namespace Fabrikam.EnterprisePizza.Reporting.Batch
{
    internal static class Program
    {
        private static void Main()
        {
            var coordinator = new DispatchCoordinator();
            Console.WriteLine("Dispatch snapshot count: " + coordinator.GetActiveTickets("014").Count);
        }
    }
}
