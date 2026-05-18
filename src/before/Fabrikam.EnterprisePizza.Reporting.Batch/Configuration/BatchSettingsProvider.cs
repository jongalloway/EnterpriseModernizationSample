using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Configuration
{
    public class BatchSettingsProvider
    {
        public IList<BatchJobDefinition> LoadNightlyJobs()
        {
            var configuredJobs = ConfigurationManager.AppSettings["Batch.NightlyJobs"];
            if (string.IsNullOrWhiteSpace(configuredJobs))
            {
                return new List<BatchJobDefinition>
                {
                    CreateDefinition("PartnerProfitabilityReports")
                };
            }

            return configuredJobs
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(jobName => jobName.Trim())
                .Where(jobName => !string.IsNullOrWhiteSpace(jobName))
                .Select(CreateDefinition)
                .ToList();
        }

        private static BatchJobDefinition CreateDefinition(string name)
        {
            return new BatchJobDefinition
            {
                Name = name,
                Schedule = ConfigurationManager.AppSettings[string.Format("Batch.Job.{0}.Schedule", name)] ?? "Nightly@01:15",
                Enabled = ReadBoolean(name, "Enabled", true)
            };
        }

        private static bool ReadBoolean(string jobName, string settingName, bool fallback)
        {
            bool parsed;
            return bool.TryParse(ConfigurationManager.AppSettings[string.Format("Batch.Job.{0}.{1}", jobName, settingName)], out parsed) ? parsed : fallback;
        }
    }
}
