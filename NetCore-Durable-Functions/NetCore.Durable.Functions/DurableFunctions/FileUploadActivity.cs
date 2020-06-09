using System.IO;
using System.Threading.Tasks;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore.Durable.Functions.Dto;

namespace NetCore.Durable.Functions.DurableFunctions
{
    public class FileUploadActivity
    {
        private readonly StorageOptions _options;

        public FileUploadActivity(IOptions<StorageOptions> options)
        {
            _options = options.Value;
        }

        [FunctionName(nameof(FileUploadActivity))]
        public async Task<ModelResult> Run([ActivityTrigger] UploadDto uploadDto, ILogger log)
        {
            log.LogInformation("Upload function started.");

            var container = new BlobContainerClient(_options.StorageConnectionString, _options.ContainerName);

            await container.CreateIfNotExistsAsync();

            var fileName = $"{uploadDto.FileName}.pdf";

            var blob = container.GetBlobClient(fileName);

            using (var document = new Document())
            {
                var page = document.Pages.Add();

                page.Paragraphs.Add(new TextFragment(uploadDto.SourceText));

                await using var pdfMs = new MemoryStream();
                document.Save(pdfMs);
                await blob.UploadAsync(pdfMs);
            }

            log.LogInformation(fileName);

            return new ModelResult("File created", fileName);
        }
    }

    public class ModelResult
    {
        private string v;
        private string fileName;

        public ModelResult(string v, string fileName)
        {
            this.v = v;
            this.fileName = fileName;
        }
    }
}