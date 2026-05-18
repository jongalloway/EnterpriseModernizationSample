using Fabrikam.EnterprisePizza.Core.Domain;

namespace Fabrikam.EnterprisePizza.Data.Repositories.StoreOps
{
    public interface IPosImportBatchRepository
    {
        PosImportBatchSnapshot GetLatestBatch(string storeNumber);
    }
}
