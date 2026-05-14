using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.IntegrationStubs
{
    internal static class ScenarioStub
    {
        public static void Pending(string scenario, string blocker)
        {
            Assert.Inconclusive("Scenario stub: " + scenario + " Pending because " + blocker);
        }
    }
}
