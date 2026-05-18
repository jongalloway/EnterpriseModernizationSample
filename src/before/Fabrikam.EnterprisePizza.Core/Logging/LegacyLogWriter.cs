using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fabrikam.EnterprisePizza.Core.Configuration;

namespace Fabrikam.EnterprisePizza.Core.Logging
{
    public sealed class LegacyLogEntry
    {
        public LegacyLogEntry(DateTime loggedAtUtc, string category, string message, TraceEventType severity, Exception exception)
        {
            LoggedAtUtc = loggedAtUtc;
            Category = category;
            Message = message;
            Severity = severity;
            Exception = exception;
        }

        public DateTime LoggedAtUtc { get; private set; }

        public string Category { get; private set; }

        public string Message { get; private set; }

        public TraceEventType Severity { get; private set; }

        public Exception Exception { get; private set; }
    }

    public sealed class LegacyLogWriter
    {
        private readonly List<LegacyLogEntry> entries;

        public LegacyLogWriter(string defaultCategory, string traceSourceName)
        {
            DefaultCategory = string.IsNullOrWhiteSpace(defaultCategory) ? "Operations" : defaultCategory;
            TraceSourceName = string.IsNullOrWhiteSpace(traceSourceName) ? "legacy-enterprise" : traceSourceName;
            entries = new List<LegacyLogEntry>();
        }

        public string DefaultCategory { get; private set; }

        public string TraceSourceName { get; private set; }

        public IList<LegacyLogEntry> Entries
        {
            get { return entries.AsReadOnly(); }
        }

        public void Write(string message, string category, TraceEventType severity, Exception exception)
        {
            var entry = new LegacyLogEntry(
                DateTime.UtcNow,
                string.IsNullOrWhiteSpace(category) ? DefaultCategory : category,
                message ?? string.Empty,
                severity,
                exception);

            entries.Add(entry);
            Trace.WriteLine("[" + entry.Severity + "] " + entry.Category + " :: " + entry.Message, TraceSourceName);
        }

        public static LegacyLogWriter Create(EnterpriseLibraryConfigurationReader configurationReader)
        {
            if (configurationReader == null)
            {
                return new LegacyLogWriter("Operations", "legacy-enterprise");
            }

            var settings = configurationReader.ReadLogWriter(configurationReader.GetDefaultLogWriterName("Operations"));
            return new LegacyLogWriter(settings.Category, settings.TraceSource);
        }
    }

    public static class LogWriter
    {
        private static readonly object SyncRoot = new object();
        private static LegacyLogWriter current = new LegacyLogWriter("Operations", "legacy-enterprise");

        public static LegacyLogWriter Current
        {
            get { return current; }
        }

        public static void Configure(EnterpriseLibraryConfigurationReader configurationReader)
        {
            SetCurrent(LegacyLogWriter.Create(configurationReader));
        }

        public static void SetCurrent(LegacyLogWriter logWriter)
        {
            if (logWriter == null)
            {
                throw new ArgumentNullException(nameof(logWriter));
            }

            lock (SyncRoot)
            {
                current = logWriter;
            }
        }

        public static void Write(string message, string category, TraceEventType severity, Exception exception)
        {
            Current.Write(message, category, severity, exception);
        }
    }
}
