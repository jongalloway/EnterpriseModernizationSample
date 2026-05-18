using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps
{
    [DataContract]
    public class OrderValidationResult
    {
        public OrderValidationResult()
        {
            Issues = new List<OrderValidationIssue>();
        }

        [DataMember(Order = 1)]
        public bool IsValid { get; set; }

        [DataMember(Order = 2)]
        public List<OrderValidationIssue> Issues { get; set; }
    }
}
