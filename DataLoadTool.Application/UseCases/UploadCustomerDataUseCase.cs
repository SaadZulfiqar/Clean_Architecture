using DataLoadTool.Application.Utilities;
using DataLoadTool.Core.Entities;
using DataLoadTool.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DataLoadTool.Application.UseCases
{
    public class UploadCustomerDataUseCase : IUploadCustomerDataUseCase
    {
        private readonly IUploadService _uploadService;
        private readonly IBatchService _batchService;
        private readonly IBatchFileService _batchFileService;
        private readonly ILogger<UploadCustomerDataUseCase> _logger;
        private readonly IValidateCustomerDataUseCase _validationService;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public UploadCustomerDataUseCase(IUploadService uploadService, IValidateCustomerDataUseCase validationService, IBatchService batchService, IBatchFileService batchFileService, ILogger<UploadCustomerDataUseCase> logger, ITokenService tokenService, IUserService userService)
        {
            _uploadService = uploadService;
            _batchService = batchService;
            _batchFileService = batchFileService;
            _logger = logger;
            _validationService = validationService;
            _tokenService = tokenService;
            _userService = userService;
        }
        public async Task Upload(List<IFormFile> files, string tenantId)
        {
            string email = _tokenService.GetEmailFromToken();
            ValidateInputs(files, tenantId, email);

            // Validate user and tenant relationship
            await ValidateUserTenantRelationshipAsync(email, tenantId);

            // Create batch and folder structure
            var batch = await CreateBatchAsync(tenantId, email);
            await CreateFolderStructureAsync(tenantId, batch.Id);

            // Process files concurrently
            var batchFileIds = new List<string>();
            var fileTasks = files.Select(async file =>
            {
                var batchFile = await ProcessFileAsync(file, tenantId, batch, email);
                batchFileIds.Add(batchFile.Id);
            });
            await Task.WhenAll(fileTasks);

            // Update batch files and save only once
            batch.Files.AddRange(batchFileIds);
            await _batchService.Save(batch);

            // Proceed to next steps like transformation and validation
            await PerformPostProcessingAsync(batch);
        }
        private async Task PerformPostProcessingAsync(Batch batch)
        {
            // Perform transformation and validation steps
            //await TransformBatchAsync(batch);
            //await ValidateBatchAsync(batch);
        }
        private void ValidateInputs(List<IFormFile> files, string tenantId, string email)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("No files were uploaded.");
            }

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentException("tenantId is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("email is required.");
            }
        }
        private async Task<Batch> CreateBatchAsync(string tenantId, string email)
        {
            var batch = new Batch
            {
                Tenant_Id = tenantId,
                Id = UniqueGuidGenerator.GenerateUniqueGuid(),
                CreatedBy = email,
                CreatedDate = UtcDateTimeGenerator.GenerateUtcDateTime()
            };
            await _batchService.Save(batch);
            _logger.LogInformation($"Batch created with Id: {batch.Id} for tenant: {tenantId}");

            return batch;
        }
        private async Task CreateFolderStructureAsync(string tenantId, string batchId)
        {
            string tenantFolder = $"{tenantId}/";
            string productFilesFolder = $"{tenantFolder}{FileDataTypes.CUSTOMER.ToFriendlyString()}/Batches/";
            string dataFolder = $"{productFilesFolder}{batchId}/";

            await EnsureFoldersExistAsync(tenantFolder, productFilesFolder, dataFolder);
        }
        private async Task EnsureFoldersExistAsync(params string[] folderPaths)
        {
            foreach (var folderPath in folderPaths)
            {
                await _uploadService.EnsureFolderExistsAsync(folderPath);
                _logger.LogInformation($"Folder ensured in S3: {folderPath}");
            }
        }
        private async Task<BatchFile> ProcessFileAsync(IFormFile file, string tenantId, Batch batch, string email)
        {
            var batchFile = await CreateBatchFileAsync(file.FileName, tenantId, batch.Id, email);
            return await UploadFileToS3Async(file, batchFile);
        }
        private async Task<BatchFile> CreateBatchFileAsync(string fileName, string tenantId, string batchId, string email)
        {
            string filePath = $"{tenantId}/{FileDataTypes.CUSTOMER.ToFriendlyString()}/Batches/{batchId}/{fileName}";

            var batchFile = new BatchFile
            {
                Tenant_Id = tenantId,
                Batch_Id = batchId,
                Id = UniqueGuidGenerator.GenerateUniqueGuid(),
                FilePath = filePath,
                Status = BatchFileStatusTypes.UploadInProgress.ToFriendlyString(),
                CreatedBy = email,
                CreatedDate = UtcDateTimeGenerator.GenerateUtcDateTime()
            };

            await _batchFileService.Save(batchFile);
            _logger.LogInformation($"Batch file created with Id: {batchFile.Id}, File Path: {filePath}");

            return batchFile;
        }
        private async Task<BatchFile> UploadFileToS3Async(IFormFile file, BatchFile batchFile)
        {
            try
            {
                using var fileStream = file.OpenReadStream();
                await _uploadService.UploadToS3Async(batchFile.FilePath, fileStream);
                _logger.LogInformation($"File uploaded to S3: {batchFile.FilePath}");

                batchFile.Status = BatchFileStatusTypes.UploadCompleted.ToFriendlyString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload file: {batchFile.FilePath}");
                batchFile.Status = BatchFileStatusTypes.UploadFailed.ToFriendlyString();
                batchFile.Details = ex.Message;
            }
            finally
            {
                await _batchFileService.Save(batchFile);
            }
            return batchFile;
        }
        private async Task ValidateUserTenantRelationshipAsync(string email, string tenantId)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with email: {email} does not exist.");
            }

            if (user.Tenant_Id != tenantId)
            {
                throw new UnauthorizedAccessException($"User: {email} is not authorized to upload files for tenant: {tenantId}");
            }
        }
    }
}
