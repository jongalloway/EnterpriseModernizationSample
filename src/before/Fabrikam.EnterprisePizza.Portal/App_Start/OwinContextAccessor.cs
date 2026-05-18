using System.Collections.Generic;
using System.Web;
using Microsoft.Owin;

namespace Fabrikam.EnterprisePizza.Portal
{
    internal static class OwinContextAccessor
    {
        public static IOwinContext Create(HttpContext context)
        {
            return new OwinContext((IDictionary<string, object>)context.Items["owin.Environment"]);
        }
    }
}
