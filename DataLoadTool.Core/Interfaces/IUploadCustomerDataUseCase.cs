using Microsoft.AspNetCore.Http;

namespace DataLoadTool.Core.Interfaces
{
    public interface IUploadCustomerDataUseCase
    {
        Task Upload(List<IFormFile> files, string tenantId);
    }
}
