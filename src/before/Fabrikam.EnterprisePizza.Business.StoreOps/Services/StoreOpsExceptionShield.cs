using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Fabrikam.EnterprisePizza.Business.StoreOps.Services
{
    public class StoreOpsExceptionShield
    {
        public const string OrderProcessingPolicy = "StoreOps.OrderProcessing";
        public const string PosWorkflowPolicy = "StoreOps.PosWorkflow";
        public const string OrderStatusPolicy = "StoreOps.OrderStatus";

        private readonly ExceptionManager _exceptionManager;

        public StoreOpsExceptionShield()
            : this(CreateDefaultManager())
        {
        }

        public StoreOpsExceptionShield(ExceptionManager exceptionManager)
        {
            _exceptionManager = exceptionManager ?? throw new ArgumentNullException(nameof(exceptionManager));
        }

        public T Execute<T>(Func<T> operation, string policyName)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            try
            {
                return operation();
            }
            catch (StoreOpsWorkflowException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _exceptionManager.HandleException(ex, policyName);
                throw;
            }
        }

        private static ExceptionManager CreateDefaultManager()
        {
            var policies = new Dictionary<string, ExceptionPolicyDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                { OrderProcessingPolicy, CreatePolicy(OrderProcessingPolicy, "Order processing failed inside the StoreOps pipeline.") },
                { PosWorkflowPolicy, CreatePolicy(PosWorkflowPolicy, "POS workflow failed inside the StoreOps lane.") },
                { OrderStatusPolicy, CreatePolicy(OrderStatusPolicy, "Order state transition failed inside StoreOps.") }
            };

            return new ExceptionManager(policies);
        }

        private static ExceptionPolicyDefinition CreatePolicy(string policyName, string message)
        {
            return new ExceptionPolicyDefinition(policyName, new[]
            {
                new ExceptionPolicyEntry(typeof(Exception), PostHandlingAction.ThrowNewException, new IExceptionHandler[]
                {
                    new WrapHandler(message, typeof(StoreOpsWorkflowException))
                })
            });
        }
    }

    [Serializable]
    public class StoreOpsWorkflowException : ApplicationException
    {
        public StoreOpsWorkflowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
