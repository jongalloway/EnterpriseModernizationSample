using System;

namespace Fabrikam.EnterprisePizza.Core.Domain
{
    public class DeliveryPromise
    {
        public FulfillmentLane Lane { get; set; }

        public DateTime QuotedReadyTime { get; set; }

        public int MinimumLeadMinutes { get; set; }

        public bool ExpediteKitchenRouting { get; set; }
    }
}
