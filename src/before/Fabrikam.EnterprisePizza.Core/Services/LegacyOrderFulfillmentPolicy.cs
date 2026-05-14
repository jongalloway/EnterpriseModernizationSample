using System;
using Fabrikam.EnterprisePizza.Core.Domain;

namespace Fabrikam.EnterprisePizza.Core.Services
{
    public class LegacyOrderFulfillmentPolicy : IOrderFulfillmentPolicy
    {
        private const int BaseLeadMinutes = 18;
        private const int CorporateRoutingMinutes = 22;
        private const int ExtendedRadiusMinutes = 12;

        private readonly IBusinessClock _clock;

        public LegacyOrderFulfillmentPolicy(IBusinessClock clock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public DeliveryPromise CreatePromise(OrderSummary order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var currentTime = _clock.GetCurrentTime();
            var minimumLeadMinutes = BaseLeadMinutes;
            var lane = FulfillmentLane.StandardMakeLine;

            if (order.IsCorporateAccount || order.Channel == OrderChannel.CorporateCatering)
            {
                minimumLeadMinutes += CorporateRoutingMinutes;
                lane = FulfillmentLane.CorporateAccountDesk;
            }

            if (order.DeliveryMileage >= 7.5m)
            {
                minimumLeadMinutes += ExtendedRadiusMinutes;
                lane = FulfillmentLane.ExtendedRadiusDispatch;
            }

            if (order.NeededBy > currentTime)
            {
                minimumLeadMinutes = Math.Max(minimumLeadMinutes, (int)Math.Ceiling((order.NeededBy - currentTime).TotalMinutes));
            }

            return new DeliveryPromise
            {
                Lane = lane,
                QuotedReadyTime = currentTime.AddMinutes(minimumLeadMinutes),
                MinimumLeadMinutes = minimumLeadMinutes,
                ExpediteKitchenRouting = order.IsCorporateAccount || order.OrderTotal >= 125m
            };
        }
    }
}
