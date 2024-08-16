using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface IValidateCustomerDataUseCase
    {
        Task ValidateCustomerDataAsync(Batch batch, BatchFile batchFile);
    }
}
