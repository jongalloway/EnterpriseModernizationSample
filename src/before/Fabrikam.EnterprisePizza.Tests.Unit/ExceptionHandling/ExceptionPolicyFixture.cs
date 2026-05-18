using System;
using System.Collections.Specialized;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Core.Logging;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.ExceptionHandling
{
    [TestFixture]
    public class ExceptionPolicyFixture
    {
        [Test]
        public void HandleException_wraps_and_logs_when_policy_requests_service_boundary_behavior()
        {
            var settings = new NameValueCollection
            {
                ["enterpriseLibrary:defaultLogWriter"] = "Operations",
                ["enterpriseLibrary:defaultExceptionPolicy"] = "ServiceBoundaryPolicy",
                ["enterpriseLibrary:logWriter:Operations"] = "category=DispatchHost;traceSource=legacy-services.dispatch",
                ["enterpriseLibrary:exceptionPolicy:ServiceBoundaryPolicy"] = "action=Wrap;exceptionType=System.InvalidOperationException, System;message=The dispatch board could not be generated.;logCategory=DispatchHost"
            };
            var reader = new EnterpriseLibraryConfigurationReader(settings);

            LogWriter.Configure(reader);
            ExceptionPolicy.Configure(reader);

            Exception wrappedException;
            var shouldRethrow = ExceptionPolicy.HandleException(new ApplicationException("boom"), "ServiceBoundaryPolicy", out wrappedException);

            Assert.That(shouldRethrow, Is.True);
            Assert.That(wrappedException, Is.TypeOf<InvalidOperationException>());
            Assert.That(wrappedException.Message, Is.EqualTo("The dispatch board could not be generated."));
            Assert.That(wrappedException.InnerException, Is.TypeOf<ApplicationException>());
            Assert.That(LogWriter.Current.Entries, Has.Count.EqualTo(1));
            Assert.That(LogWriter.Current.Entries[0].Category, Is.EqualTo("DispatchHost"));
        }
    }
}
