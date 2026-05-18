using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class PosReceipt
    {
        public PosReceipt()
        {
            Lines = new List<string>();
        }

        [DataMember(Order = 1)]
        public int OrderNumber { get; set; }

        [DataMember(Order = 2)]
        public string StoreNumber { get; set; }

        [DataMember(Order = 3)]
        public string ReceiptNumber { get; set; }

        [DataMember(Order = 4)]
        public string CustomerName { get; set; }

        [DataMember(Order = 5)]
        public string CashierId { get; set; }

        [DataMember(Order = 6)]
        public DateTime GeneratedAtLocal { get; set; }

        [DataMember(Order = 7)]
        public List<string> Lines { get; set; }

        [DataMember(Order = 8)]
        public decimal Total { get; set; }
    }
}
