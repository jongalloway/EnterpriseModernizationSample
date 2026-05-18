using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Fabrikam.EnterprisePizza.Core.Logging;

namespace Fabrikam.EnterprisePizza.Core.ExceptionHandling
{
    public static class ExceptionPolicy
    {
        public static bool HandleException(Exception exception, string policyName, out Exception exceptionToThrow)
        {
            exceptionToThrow = exception;
            return exceptionToThrow != null;
        private static readonly object SyncRoot = new object();
        private static readonly IDictionary<string, LegacyExceptionPolicyDefinition> policies = new Dictionary<string, LegacyExceptionPolicyDefinition>(StringComparer.OrdinalIgnoreCase);
        private static string defaultPolicyName = "ServiceBoundaryPolicy";

        public static void Configure(EnterpriseLibraryConfigurationReader configurationReader)
        {
            if (configurationReader == null)
            {
                throw new ArgumentNullException(nameof(configurationReader));
            }

            var configuredPolicy = configurationReader.ReadExceptionPolicy(configurationReader.GetDefaultExceptionPolicyName("ServiceBoundaryPolicy"));
            SetPolicies(new[] { configuredPolicy }, configuredPolicy.Name);
        }

        public static void SetPolicies(IEnumerable<LegacyExceptionPolicyDefinition> configuredPolicies, string defaultPolicy)
        {
            lock (SyncRoot)
            {
                policies.Clear();
                if (configuredPolicies != null)
                {
                    foreach (var configuredPolicy in configuredPolicies)
                    {
                        if (configuredPolicy == null || string.IsNullOrWhiteSpace(configuredPolicy.Name))
                        {
                            continue;
                        }

                        policies[configuredPolicy.Name] = configuredPolicy;
                    }
                }

                defaultPolicyName = string.IsNullOrWhiteSpace(defaultPolicy) ? "ServiceBoundaryPolicy" : defaultPolicy;
            }
        }

        public static bool HandleException(Exception exceptionToHandle, string policyName, out Exception exceptionToThrow)
        {
            if (exceptionToHandle == null)
            {
                throw new ArgumentNullException(nameof(exceptionToHandle));
            }

            LegacyExceptionPolicyDefinition policy;
            lock (SyncRoot)
            {
                if (!policies.TryGetValue(string.IsNullOrWhiteSpace(policyName) ? defaultPolicyName : policyName, out policy))
                {
                    exceptionToThrow = exceptionToHandle;
                    return true;
                }
            }

            if (!string.IsNullOrWhiteSpace(policy.LogCategory))
            {
                LogWriter.Write(exceptionToHandle.Message, policy.LogCategory, TraceEventType.Error, exceptionToHandle);
            }

            switch (policy.Action)
            {
                case LegacyExceptionPolicyAction.Swallow:
                    exceptionToThrow = null;
                    return false;
                case LegacyExceptionPolicyAction.Wrap:
                    exceptionToThrow = CreateWrappedException(policy, exceptionToHandle);
                    return true;
                default:
                    exceptionToThrow = exceptionToHandle;
                    return true;
            }
        }

        private static Exception CreateWrappedException(LegacyExceptionPolicyDefinition policy, Exception innerException)
        {
            try
            {
                var exceptionType = Type.GetType(policy.ExceptionTypeToken, false) ?? typeof(InvalidOperationException);
                return (Exception)Activator.CreateInstance(exceptionType, policy.WrapMessage, innerException);
            }
            catch
            {
                return new InvalidOperationException(policy.WrapMessage, innerException);
            }
        }
    }
}
