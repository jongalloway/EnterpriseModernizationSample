using System;
using System.IO;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    internal static class LegacyServiceSmokePaths
    {
        private static readonly string BeforeRoot = FindBeforeRoot();

        public static string Combine(params string[] segments)
        {
            var allSegments = new string[segments.Length + 1];
            allSegments[0] = BeforeRoot;
            Array.Copy(segments, 0, allSegments, 1, segments.Length);
            return Path.Combine(allSegments);
        }

        private static string FindBeforeRoot()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(LegacyServiceSmokePaths).Assembly.Location);
            var current = new DirectoryInfo(assemblyDirectory ?? AppDomain.CurrentDomain.BaseDirectory);

            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Fabrikam.EnterprisePizza.Legacy.sln")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Could not locate the src\\before root for legacy service smoke tests.");
        }
    }
}
