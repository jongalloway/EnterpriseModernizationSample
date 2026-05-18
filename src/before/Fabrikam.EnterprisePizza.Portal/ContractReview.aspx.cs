using System.Web.Script.Services;
using System.Web.Services;

namespace Fabrikam.EnterprisePizza.Portal
{
    public partial class ContractReview : System.Web.UI.Page
    {
        [WebMethod]
        [ScriptMethod]
        public static string[] SearchPartners(string prefixText, int count)
        {
            return new PartnerManagementRepository().SearchPartnerNames(prefixText, count);
        }
    }
}
