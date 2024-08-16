using DataLoadTool.Core.Entities;

namespace DataLoadTool.Core.Interfaces
{
    public interface ITransformCustomerDataUseCase
    {
        Task TransformCustomerDataAsync(Batch batch, BatchFile batchFile);
    }
}
