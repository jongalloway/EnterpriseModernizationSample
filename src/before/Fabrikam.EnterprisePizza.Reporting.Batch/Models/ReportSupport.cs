using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Models
{
    public interface IReportParameterModel
    {
        IEnumerable<ReportParameterValue> ToReportParameters();
    }

    public sealed class ReportParameterValue
    {
        public ReportParameterValue(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter name is required.", nameof(name));
            }

            Name = name.Trim();
            Value = value ?? string.Empty;
        }

        public string Name { get; }

        public string Value { get; }
    }

    public sealed class ReportDefinition
    {
        public ReportDefinition(string key, string displayName, string reportPath, string localDefinitionPath, string sharedDataSourcePath)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Report key is required.", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name is required.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(reportPath))
            {
                throw new ArgumentException("Report path is required.", nameof(reportPath));
            }

            if (string.IsNullOrWhiteSpace(localDefinitionPath))
            {
                throw new ArgumentException("Local definition path is required.", nameof(localDefinitionPath));
            }

            if (string.IsNullOrWhiteSpace(sharedDataSourcePath))
            {
                throw new ArgumentException("Shared data source path is required.", nameof(sharedDataSourcePath));
            }

            Key = key.Trim();
            DisplayName = displayName.Trim();
            ReportPath = reportPath.Trim();
            LocalDefinitionPath = localDefinitionPath.Trim();
            SharedDataSourcePath = sharedDataSourcePath.Trim();
        }

        public string Key { get; }

        public string DisplayName { get; }

        public string ReportPath { get; }

        public string LocalDefinitionPath { get; }

        public string SharedDataSourcePath { get; }
    }

    public sealed class ReportExecutionRequest
    {
        public ReportExecutionRequest(Uri executionEndpoint, string reportPath, string renderFormat, IEnumerable<ReportParameterValue> parameters, int timeoutSeconds)
        {
            if (executionEndpoint == null)
            {
                throw new ArgumentNullException(nameof(executionEndpoint));
            }

            if (string.IsNullOrWhiteSpace(reportPath))
            {
                throw new ArgumentException("Report path is required.", nameof(reportPath));
            }

            if (string.IsNullOrWhiteSpace(renderFormat))
            {
                throw new ArgumentException("Render format is required.", nameof(renderFormat));
            }

            if (timeoutSeconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), "Timeout must be at least one second.");
            }

            ExecutionEndpoint = executionEndpoint;
            ReportPath = reportPath.Trim();
            RenderFormat = renderFormat.Trim().ToUpperInvariant();
            Parameters = new ReadOnlyCollection<ReportParameterValue>((parameters ?? Enumerable.Empty<ReportParameterValue>()).ToList());
            TimeoutSeconds = timeoutSeconds;
        }

        public Uri ExecutionEndpoint { get; }

        public string ReportPath { get; }

        public string RenderFormat { get; }

        public ReadOnlyCollection<ReportParameterValue> Parameters { get; }

        public int TimeoutSeconds { get; }
    }
}
