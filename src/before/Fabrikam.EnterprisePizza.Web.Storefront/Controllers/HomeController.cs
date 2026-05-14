using System.Web.Mvc;

namespace Fabrikam.EnterprisePizza.Web.Storefront.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Banner = "Extreme Value Tuesday";
            return View();
        }
    }
}
