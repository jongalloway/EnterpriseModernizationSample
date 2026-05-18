using System;

namespace Fabrikam.EnterprisePizza.Core.Domain
{
    public class PosImportBatchSnapshot
    {
        public int PosOrderImportBatchId { get; set; }

        public string StoreNumber { get; set; }

        public string SourceSystem { get; set; }

        public DateTime BatchDate { get; set; }

        public DateTime ImportedUtc { get; set; }

        public string BatchStatus { get; set; }

        public int ItemCount { get; set; }
    }
}
