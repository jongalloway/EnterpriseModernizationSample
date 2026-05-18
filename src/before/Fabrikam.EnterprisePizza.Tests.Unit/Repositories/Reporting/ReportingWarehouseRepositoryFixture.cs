using System;
using Fabrikam.EnterprisePizza.Data.DataSets.Reporting;
using Fabrikam.EnterprisePizza.Data.Gateways;
using Fabrikam.EnterprisePizza.Data.Repositories.Reporting;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.Reporting
{
    [TestFixture]
    public class ReportingWarehouseRepositoryFixture
    {
        [Test]
        public void Dimensional_repositories_return_reporting_dimension_rows_from_reporting_connection_catalog()
        {
            var gateway = new LegacyDbGateway();
            var dimStore = new DimStoreRepository(gateway).GetStores("Puget Sound", false);
            var dimDriver = new DimDriverRepository(gateway).GetDrivers("014", false);
            var dimPartner = new DimPartnerRepository(gateway).GetPartners(null, false);
            var dimTime = new DimTimeRepository(gateway).GetCalendarRange(new DateTime(2026, 5, 12), new DateTime(2026, 5, 13));

            Assert.That(dimStore.DimStore.Rows, Has.Count.EqualTo(2));
            Assert.That(dimStore.DimStore.Rows[0]["StoreNumber"], Is.EqualTo("014"));
            Assert.That(dimDriver.DimDriver.Rows, Has.Count.EqualTo(2));
            Assert.That(dimDriver.DimDriver.Rows[0]["DriverCode"], Is.EqualTo("DRV-17"));
            Assert.That(dimPartner.DimPartner.Rows, Has.Count.EqualTo(2));
            Assert.That(dimPartner.DimPartner.Rows[1]["PartnerCode"], Is.EqualTo("PART-200"));
            Assert.That(dimTime.DimTime.Rows, Has.Count.EqualTo(2));
            Assert.That(dimTime.DimTime.Rows[0]["DateKey"], Is.EqualTo(20260512));
        }

        [Test]
        public void Fact_repositories_return_delivery_order_and_partner_revenue_rows_for_reporting_date_grain()
        {
            var gateway = new LegacyDbGateway();
            var factDelivery = new FactDeliveryRepository(gateway).GetDailyDeliveries("014", new DateTime(2026, 5, 12));
            var factOrder = new FactOrderRepository(gateway).GetDailyOrders("014", new DateTime(2026, 5, 12));
            var factPartnerRevenue = new FactPartnerRevenueRepository(gateway).GetMonthlyPartnerRevenue("PART-100", new DateTime(2026, 5, 1));

            Assert.That(factDelivery.FactDelivery.Rows, Has.Count.EqualTo(2));
            Assert.That(factDelivery.FactDelivery.Rows[0]["OrderNumber"], Is.EqualTo("DEL-14012"));
            Assert.That(factOrder.FactOrder.Rows, Has.Count.EqualTo(2));
            Assert.That(factOrder.FactOrder.Rows[1]["OrderChannel"], Is.EqualTo("Phone"));
            Assert.That(factPartnerRevenue.FactPartnerRevenue.Rows, Has.Count.EqualTo(1));
            Assert.That(factPartnerRevenue.FactPartnerRevenue.Rows[0]["SettlementAmount"], Is.EqualTo(254.77m));
        }

        [Test]
        public void Star_schema_repository_joins_dimension_labels_onto_fact_result_sets()
        {
            var repository = new ReportingStarSchemaRepository(new LegacyDbGateway());

            var deliveryStar = repository.GetDeliveryStoreDriverSnapshot("014", new DateTime(2026, 5, 12));
            var orderStar = repository.GetOrderChannelMixSnapshot("014", new DateTime(2026, 5, 12));
            var revenueStar = repository.GetPartnerRevenueSettlementSnapshot("PART-100", new DateTime(2026, 5, 1));

            Assert.That(deliveryStar.DeliveryStoreDriverSnapshot.Rows, Has.Count.EqualTo(2));
            Assert.That(deliveryStar.DeliveryStoreDriverSnapshot.Rows[0]["StoreName"], Is.EqualTo("Redmond Ridge"));
            Assert.That(deliveryStar.DeliveryStoreDriverSnapshot.Rows[0]["DriverName"], Is.EqualTo("Mario Vasquez"));
            Assert.That(orderStar.OrderChannelMixSnapshot.Rows, Has.Count.EqualTo(2));
            Assert.That(orderStar.OrderChannelMixSnapshot.Rows[0]["PartnerName"], Is.EqualTo("Contoso Office Parks"));
            Assert.That(revenueStar.PartnerRevenueSettlementSnapshot.Rows, Has.Count.EqualTo(1));
            Assert.That(revenueStar.PartnerRevenueSettlementSnapshot.Rows[0]["CalendarMonthLabel"], Is.EqualTo("2026-05"));
            Assert.That(revenueStar.PartnerRevenueSettlementSnapshot.Rows[0]["NetMargin"], Is.EqualTo(502.73m));
        }
    }
}
