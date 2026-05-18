using System;
using Fabrikam.EnterprisePizza.Data.Gateways;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Gateways
{
    [TestFixture]
    public class StoredProcedureCallFixture
    {
        private GatewayParameter[] parameters;

        [SetUp]
        public void SetUp()
        {
            parameters = new[]
            {
                new GatewayParameter("@StoreNumber", "042"),
                new GatewayParameter("@WeeksBack", 4)
            };
        }

        [TearDown]
        public void TearDown()
        {
            parameters = null;
        }

        [Test]
        public void Constructor_preserves_connection_name_procedure_alias_and_parameter_order()
        {
            var call = new StoredProcedureCall("FabrikamPizza_Reporting", "dbo.usp_WorkforceReports_GetOvertimeTrend", parameters);

            Assert.That(call.ConnectionName, Is.EqualTo("FabrikamPizza_Reporting"));
            Assert.That(call.StoredProcedureName, Is.EqualTo("dbo.usp_WorkforceReports_GetOvertimeTrend"));
            Assert.That(call.Parameters, Has.Count.EqualTo(2));
            Assert.That(call.Parameters[0].Name, Is.EqualTo("@StoreNumber"));
            Assert.That(call.Parameters[1].Value, Is.EqualTo(4));
        }

        [Test]
        public void Constructor_rejects_blank_connection_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => new StoredProcedureCall(" ", "dbo.usp_DispatchBoard_GetActiveTickets"));

            Assert.That(ex.ParamName, Is.EqualTo("connectionName"));
        }

        [Test]
        public void Constructor_rejects_blank_procedure_name()
        {
            var ex = Assert.Throws<ArgumentException>(() => new StoredProcedureCall("FabrikamPizza_StoreOps", string.Empty));

            Assert.That(ex.ParamName, Is.EqualTo("procedureName"));
        }
    }
}
