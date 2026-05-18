using System;
using System.Diagnostics;
using System.Globalization;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Logging
{
    public class LegacyBatchLogger
    {
        public void Info(string message, params object[] args)
        {
            Write("INFO", message, args);
        }

        public void Warn(string message, params object[] args)
        {
            Write("WARN", message, args);
        }

        public void Error(string message, params object[] args)
        {
            Write("ERROR", message, args);
        }

        private static void Write(string level, string message, params object[] args)
        {
            var formatted = args == null || args.Length == 0
                ? message
                : string.Format(CultureInfo.InvariantCulture, message, args);
            var line = string.Format(
                CultureInfo.InvariantCulture,
                "[{0:u}] {1} {2}",
                DateTime.UtcNow,
                level.PadRight(5),
                formatted);

            Trace.WriteLine(line);
            Console.WriteLine(line);
        }
    }
}
