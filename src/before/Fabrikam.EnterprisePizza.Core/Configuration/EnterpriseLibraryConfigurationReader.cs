using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Composition;

namespace Fabrikam.EnterprisePizza.Core.Configuration
{
    public sealed class EnterpriseLibraryConfigurationReader
    {
        private const string Prefix = "enterpriseLibrary:";
        private readonly NameValueCollection appSettings;
        private readonly ConnectionStringSettingsCollection connectionStrings;

        public EnterpriseLibraryConfigurationReader()
            : this(ConfigurationManager.AppSettings, ConfigurationManager.ConnectionStrings)
        {
        }

        public EnterpriseLibraryConfigurationReader(NameValueCollection appSettings)
            : this(appSettings, new ConnectionStringSettingsCollection())
        {
        }

        public EnterpriseLibraryConfigurationReader(NameValueCollection appSettings, ConnectionStringSettingsCollection connectionStrings)
        {
            this.appSettings = appSettings ?? new NameValueCollection();
            this.connectionStrings = connectionStrings ?? new ConnectionStringSettingsCollection();
        }

        public string GetDefaultContainerName(string fallback)
        {
            return GetRequiredValue(Prefix + "defaultContainer", fallback);
        }

        public string GetDefaultLogWriterName(string fallback)
        {
            return GetRequiredValue(Prefix + "defaultLogWriter", fallback);
        }

        public string GetDefaultExceptionPolicyName(string fallback)
        {
            return GetRequiredValue(Prefix + "defaultExceptionPolicy", fallback);
        }

        public IEnumerable<LegacyComponentRegistration> ReadContainerRegistrations(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                return Enumerable.Empty<LegacyComponentRegistration>();
            }

            var prefix = Prefix + "container:" + containerName + ":";
            return appSettings.AllKeys
                .Where(key => key != null && key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .Select(key => ParseRegistration(appSettings[key]))
                .Where(registration => registration != null)
                .ToArray();
        }

        public LegacyLogWriterSettings ReadLogWriter(string logWriterName)
        {
            var key = Prefix + "logWriter:" + (string.IsNullOrWhiteSpace(logWriterName) ? GetDefaultLogWriterName("Operations") : logWriterName.Trim());
            var values = ParseKeyValuePairs(appSettings[key]);

            return new LegacyLogWriterSettings(
                GetValue(values, "category", "Operations"),
                GetValue(values, "traceSource", "legacy-enterprise"));
        }

        public LegacyExceptionPolicyDefinition ReadExceptionPolicy(string policyName)
        {
            var resolvedPolicyName = string.IsNullOrWhiteSpace(policyName)
                ? GetDefaultExceptionPolicyName("ServiceBoundaryPolicy")
                : policyName.Trim();
            var key = Prefix + "exceptionPolicy:" + resolvedPolicyName;
            var values = ParseKeyValuePairs(appSettings[key]);

            return new LegacyExceptionPolicyDefinition(
                resolvedPolicyName,
                ParseAction(GetValue(values, "action", "Rethrow")),
                GetValue(values, "exceptionType", typeof(InvalidOperationException).AssemblyQualifiedName),
                GetValue(values, "message", "A legacy service boundary rejected the request."),
                GetValue(values, "logCategory", string.Empty));
        }

        public string GetConnectionName(string areaName, string fallback)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                return fallback;
            }

            return GetRequiredValue(Prefix + "database:" + areaName.Trim(), fallback);
        }

        public ConnectionStringSettings GetConnectionStringSettings(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return null;
            }

            return connectionStrings.Cast<ConnectionStringSettings>()
                .FirstOrDefault(settings => string.Equals(settings.Name, connectionName, StringComparison.OrdinalIgnoreCase));
        }

        public Type ResolveType(string typeToken)
        {
            if (string.IsNullOrWhiteSpace(typeToken))
            {
                throw new ArgumentException("A type token is required.", nameof(typeToken));
            }

            var resolvedToken = ResolveAlias(typeToken.Trim());
            var resolvedType = Type.GetType(resolvedToken, false);
            if (resolvedType != null)
            {
                return resolvedType;
            }

            resolvedType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(resolvedToken, false))
                .FirstOrDefault(type => type != null);
            if (resolvedType != null)
            {
                return resolvedType;
            }

            throw new InvalidOperationException("Unable to resolve configured type token '" + typeToken + "'.");
        }

        private string ResolveAlias(string typeToken)
        {
            var aliasValue = appSettings[Prefix + "typeAlias:" + typeToken];
            return string.IsNullOrWhiteSpace(aliasValue)
                ? typeToken
                : aliasValue.Trim();
        }

        private static LegacyComponentRegistration ParseRegistration(string registrationValue)
        {
            var values = ParseKeyValuePairs(registrationValue);
            var serviceType = GetValue(values, "service", string.Empty);
            var mapToType = GetValue(values, "mapTo", string.Empty);

            if (string.IsNullOrWhiteSpace(serviceType) || string.IsNullOrWhiteSpace(mapToType))
            {
                return null;
            }

            return new LegacyComponentRegistration(
                serviceType,
                mapToType,
                GetValue(values, "name", string.Empty),
                ParseLifetime(GetValue(values, "lifetime", "Transient")));
        }

        private static IDictionary<string, string> ParseKeyValuePairs(string rawValue)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return values;
            }

            var segments = rawValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < segments.Length; index++)
            {
                var parts = segments[index].Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                values[parts[0].Trim()] = parts[1].Trim();
            }

            return values;
        }

        private static string GetValue(IDictionary<string, string> values, string key, string fallback)
        {
            string value;
            return values.TryGetValue(key, out value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : fallback;
        }

        private string GetRequiredValue(string key, string fallback)
        {
            var configuredValue = appSettings[key];
            return string.IsNullOrWhiteSpace(configuredValue)
                ? fallback
                : configuredValue.Trim();
        }

        private static LegacyLifetime ParseLifetime(string lifetime)
        {
            LegacyLifetime parsedLifetime;
            return Enum.TryParse(lifetime, true, out parsedLifetime)
                ? parsedLifetime
                : LegacyLifetime.Transient;
        }

        private static LegacyExceptionPolicyAction ParseAction(string action)
        {
            LegacyExceptionPolicyAction parsedAction;
            return Enum.TryParse(action, true, out parsedAction)
                ? parsedAction
                : LegacyExceptionPolicyAction.Rethrow;
        }
    }

    public sealed class LegacyComponentRegistration
    {
        public LegacyComponentRegistration(string serviceTypeToken, string mapToTypeToken, string name, LegacyLifetime lifetime)
        {
            ServiceTypeToken = serviceTypeToken;
            MapToTypeToken = mapToTypeToken;
            Name = name;
            Lifetime = lifetime;
        }

        public string ServiceTypeToken { get; private set; }

        public string MapToTypeToken { get; private set; }

        public string Name { get; private set; }

        public LegacyLifetime Lifetime { get; private set; }
    }

    public sealed class LegacyLogWriterSettings
    {
        public LegacyLogWriterSettings(string category, string traceSource)
        {
            Category = category;
            TraceSource = traceSource;
        }

        public string Category { get; private set; }

        public string TraceSource { get; private set; }
    }

    public sealed class LegacyExceptionPolicyDefinition
    {
        public LegacyExceptionPolicyDefinition(string name, LegacyExceptionPolicyAction action, string exceptionTypeToken, string wrapMessage, string logCategory)
        {
            Name = name;
            Action = action;
            ExceptionTypeToken = exceptionTypeToken;
            WrapMessage = wrapMessage;
            LogCategory = logCategory;
        }

        public string Name { get; private set; }

        public LegacyExceptionPolicyAction Action { get; private set; }

        public string ExceptionTypeToken { get; private set; }

        public string WrapMessage { get; private set; }

        public string LogCategory { get; private set; }
    }

    public enum LegacyExceptionPolicyAction
    {
        Rethrow,
        Wrap,
        Swallow
    }
}
