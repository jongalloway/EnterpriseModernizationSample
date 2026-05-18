using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Fabrikam.EnterprisePizza.Core.Domain.Batch;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Configuration
{
    public class BatchSettingsProvider
    {
        public IList<BatchJobDefinition> LoadNightlyJobs()
        {
            var configuredJobs = ConfigurationManager.AppSettings["Batch.Jobs"];
            if (string.IsNullOrWhiteSpace(configuredJobs))
            {
                return new List<BatchJobDefinition>();
            }

            return configuredJobs
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(jobName => jobName.Trim())
                .Where(jobName => !string.IsNullOrWhiteSpace(jobName))
                .Select(ReadJobDefinition)
                .ToList();
        }

        private static BatchJobDefinition ReadJobDefinition(string jobName)
        {
            var definition = new BatchJobDefinition
            {
                Name = jobName,
                Schedule = ReadSetting(jobName, "Schedule", "Nightly@01:00"),
                SourceConnectionName = ReadSetting(jobName, "SourceConnectionName"),
                TargetConnectionName = ReadSetting(jobName, "TargetConnectionName"),
                MaxRetryCount = ReadInt32(jobName, "MaxRetryCount", 1),
                RetryIntervalSeconds = ReadInt32(jobName, "RetryIntervalSeconds", 30),
                FeedPath = ReadSetting(jobName, "FeedPath"),
                Enabled = ReadBoolean(jobName, "Enabled", true)
            };

            ValidateConnection(definition.SourceConnectionName);
            ValidateConnection(definition.TargetConnectionName);
            return definition;
        }

        private static string ReadSetting(string jobName, string settingName, string fallback = null)
        {
            return ConfigurationManager.AppSettings[string.Format("Batch.Job.{0}.{1}", jobName, settingName)] ?? fallback;
        }

        private static int ReadInt32(string jobName, string settingName, int fallback)
        {
            int parsed;
            return int.TryParse(ReadSetting(jobName, settingName), out parsed) ? parsed : fallback;
        }

        private static bool ReadBoolean(string jobName, string settingName, bool fallback)
        {
            bool parsed;
            return bool.TryParse(ReadSetting(jobName, settingName), out parsed) ? parsed : fallback;
        }

        private static void ValidateConnection(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return;
            }

            if (ConfigurationManager.ConnectionStrings[connectionName] == null)
            {
                throw new ConfigurationErrorsException("Missing batch connection string definition for '" + connectionName + "'.");
            }
        }
    }
}
