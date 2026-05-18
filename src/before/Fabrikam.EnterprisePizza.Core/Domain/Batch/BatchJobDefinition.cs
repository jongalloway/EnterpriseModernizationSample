namespace Fabrikam.EnterprisePizza.Core.Domain.Batch
{
    public class BatchJobDefinition
    {
        public string Name { get; set; }

        public string Schedule { get; set; }

        public string SourceConnectionName { get; set; }

        public string TargetConnectionName { get; set; }

        public int MaxRetryCount { get; set; }

        public int RetryIntervalSeconds { get; set; }

        public string FeedPath { get; set; }

        public bool Enabled { get; set; }
    }
}
