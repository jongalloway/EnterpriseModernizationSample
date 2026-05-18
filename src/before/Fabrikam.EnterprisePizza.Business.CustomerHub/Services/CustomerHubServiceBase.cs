using System;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public abstract class CustomerHubServiceBase
    {
        protected T Execute<T>(Func<T> operation, string operationName)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                throw HandleException(ex, operationName);
            }
        }

        protected void Execute(Action operation, string operationName)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            Execute(delegate
            {
                operation();
                return true;
            }, operationName);
        }

        private static Exception HandleException(Exception exception, string operationName)
        {
            Exception exceptionToThrow;
            var boundaryException = new InvalidOperationException("CustomerHub business operation failed: " + operationName + ".", exception);
            if (ExceptionPolicy.HandleException(boundaryException, "ServiceBoundaryPolicy", out exceptionToThrow) && exceptionToThrow != null)
            {
                return exceptionToThrow;
            }

            return boundaryException;
        }
    }
}
