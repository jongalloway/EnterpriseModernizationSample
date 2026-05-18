using System;

namespace Fabrikam.EnterprisePizza.Core.ExceptionHandling
{
    public static class ExceptionPolicy
    {
        public static bool HandleException(Exception exception, string policyName, out Exception exceptionToThrow)
        {
            exceptionToThrow = exception;
            return exceptionToThrow != null;
        }
    }
}
