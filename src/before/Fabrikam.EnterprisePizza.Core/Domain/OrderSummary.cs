using System;

namespace Fabrikam.EnterprisePizza.Core.Domain
{
    public class OrderSummary
    {
        public int OrderId { get; set; }

        public string StoreNumber { get; set; }

        public string CustomerDisplayName { get; set; }

        public OrderChannel Channel { get; set; }

        public decimal OrderTotal { get; set; }

        public bool IsCorporateAccount { get; set; }

        public decimal DeliveryMileage { get; set; }

        public DateTime NeededBy { get; set; }
    }
}
