using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Configuration
{
    public sealed class ReportingServiceConfiguration
    {
        public ReportingServiceConfiguration(Uri executionEndpoint, string reportFolder, string defaultRenderFormat, int executionTimeoutSeconds, bool useDefaultCredentials)
        {
            if (executionEndpoint == null)
            {
                throw new ArgumentNullException(nameof(executionEndpoint));
            }

            if (!executionEndpoint.IsAbsoluteUri)
            {
                throw new ArgumentException("Execution endpoint must be an absolute URI.", nameof(executionEndpoint));
            }

            if (!executionEndpoint.AbsoluteUri.EndsWith("ReportExecution2005.asmx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Execution endpoint must target the ReportExecution2005.asmx endpoint.", nameof(executionEndpoint));
            }

            if (string.IsNullOrWhiteSpace(reportFolder))
            {
                throw new ArgumentException("Report folder is required.", nameof(reportFolder));
            }

            if (string.IsNullOrWhiteSpace(defaultRenderFormat))
            {
                throw new ArgumentException("Default render format is required.", nameof(defaultRenderFormat));
            }

            if (executionTimeoutSeconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(executionTimeoutSeconds), "Execution timeout must be at least one second.");
            }

            ExecutionEndpoint = executionEndpoint;
            ReportFolder = NormalizeServerPath(reportFolder);
            DefaultRenderFormat = defaultRenderFormat.Trim().ToUpperInvariant();
            ExecutionTimeoutSeconds = executionTimeoutSeconds;
            UseDefaultCredentials = useDefaultCredentials;
        }

        public Uri ExecutionEndpoint { get; }

        public string ReportFolder { get; }

        public string DefaultRenderFormat { get; }

        public int ExecutionTimeoutSeconds { get; }

        public bool UseDefaultCredentials { get; }

        public static ReportingServiceConfiguration LoadFromAppSettings()
        {
            return new ReportingServiceConfiguration(
                new Uri(ReadAppSetting("ReportingService.ReportServerUrl", "http://reports.fabrikam.com/ReportServer/ReportExecution2005.asmx"), UriKind.Absolute),
                ReadAppSetting("ReportingService.ReportFolder", "/Fabrikam Enterprise Pizza/Operations"),
                ReadAppSetting("ReportingService.DefaultRenderFormat", "PDF"),
                ReadIntAppSetting("ReportingService.ExecutionTimeoutSeconds", 90),
                ReadBoolAppSetting("ReportingService.UseDefaultCredentials", true));
        }

        public static string NormalizeServerPath(string path)
        {
            var trimmed = (path ?? string.Empty).Trim();
            if (trimmed.Length == 0)
            {
                return string.Empty;
            }

            return trimmed.StartsWith("/", StringComparison.Ordinal) ? trimmed : "/" + trimmed;
        }

        private static string ReadAppSetting(string key, string fallback)
        {
            var configuredValue = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(configuredValue) ? fallback : configuredValue.Trim();
        }

        private static int ReadIntAppSetting(string key, int fallback)
        {
            int parsedValue;
            return int.TryParse(ConfigurationManager.AppSettings[key], out parsedValue) ? parsedValue : fallback;
        }

        private static bool ReadBoolAppSetting(string key, bool fallback)
        {
            bool parsedValue;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out parsedValue) ? parsedValue : fallback;
        }
    }

    public sealed class ReportingDataSourceConfiguration
    {
        public ReportingDataSourceConfiguration(string name, string provider, string connectionString, string sharedDataSourcePath, bool integratedSecurity, string prompt)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Data source name is required.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentException("Data source provider is required.", nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is required.", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(sharedDataSourcePath))
            {
                throw new ArgumentException("Shared data source path is required.", nameof(sharedDataSourcePath));
            }

            Name = name.Trim();
            Provider = provider.Trim();
            ConnectionString = connectionString.Trim();
            SharedDataSourcePath = ReportingServiceConfiguration.NormalizeServerPath(sharedDataSourcePath);
            IntegratedSecurity = integratedSecurity;
            Prompt = prompt ?? string.Empty;
        }

        public string Name { get; }

        public string Provider { get; }

        public string ConnectionString { get; }

        public string SharedDataSourcePath { get; }

        public bool IntegratedSecurity { get; }

        public string Prompt { get; }

        public static ReadOnlyCollection<ReportingDataSourceConfiguration> LoadFromAppSettings()
        {
            var dataSources = new List<ReportingDataSourceConfiguration>
            {
                new ReportingDataSourceConfiguration(
                    "FabrikamPizza_Reporting",
                    ReadAppSetting("ReportingService.DataSource.FabrikamPizza_Reporting.Provider", "SQL"),
                    ReadAppSetting("ReportingService.DataSource.FabrikamPizza_Reporting.ConnectionString", "Data Source=SQLLEGACY01;Initial Catalog=FabrikamPizza_Reporting;Integrated Security=SSPI;"),
                    ReadAppSetting("ReportingService.DataSource.FabrikamPizza_Reporting.SharedPath", "/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting"),
                    ReadBoolAppSetting("ReportingService.DataSource.FabrikamPizza_Reporting.IntegratedSecurity", true),
                    ReadAppSetting("ReportingService.DataSource.FabrikamPizza_Reporting.Prompt", string.Empty))
            };

            return new ReadOnlyCollection<ReportingDataSourceConfiguration>(dataSources);
        }

        private static string ReadAppSetting(string key, string fallback)
        {
            var configuredValue = ConfigurationManager.AppSettings[key];
            return configuredValue == null ? fallback : configuredValue;
        }

        private static bool ReadBoolAppSetting(string key, bool fallback)
        {
            bool parsedValue;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out parsedValue) ? parsedValue : fallback;
        }
    }
}
