using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore.Durable.Functions.Dto;

namespace NetCore.Durable.Functions.DurableFunctions
{
	public class InsertDataActivity
	{
		private readonly StorageOptions _options;

		public InsertDataActivity(IOptions<StorageOptions> options)
		{
			_options = options.Value;
		}

		[FunctionName(nameof(InsertDataActivity))]
		public async Task<UploadDto> Run([ActivityTrigger] EventDto translationDto, ILogger log)
		{
			//var storageAccount = CloudStorageAccount.Parse(_options.StorageConnectionString);

			//var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

			//// LOOKUP TABLE
			//var table = tableClient.GetTableReference("TranslationLookups");

			//var entityOperation =
			//	TableOperation.Retrieve<TranslationLookup>("Entity", translationDto.Entity.ToString());
			//var entityTypeOperation =
			//	TableOperation.Retrieve<TranslationLookup>("EntityType", translationDto.EntityType.ToString());

			//var entityResult = await table.ExecuteAsync(entityOperation).ConfigureAwait(false);
			//var entityTypeResult = await table.ExecuteAsync(entityTypeOperation).ConfigureAwait(false);

			//// MAPS TABLE
			//table = tableClient.GetTableReference("TranslationMaps");

			//var mapOperation = TableOperation
			//	.Retrieve<TranslationMapDto>(translationDto.Entity.ToString(), translationDto.EntityType.ToString());

			//var mapResult = await table.ExecuteAsync(mapOperation).ConfigureAwait(false);
			
			//// RESULTS
			//var entityLookup = entityResult.Result as TranslationLookup;
			//var entityType = entityTypeResult.Result as TranslationLookup;
			//var mapDto = mapResult.Result as TranslationMapDto;

			//var uploadDto = new UploadDto
			//{
			//	FileName = $"{entityLookup?.Description}_{entityType?.Description}_{Guid.NewGuid()}",
			//	ProjectName = mapDto?.ProjectName,
			//	Data = translationDto.Data
			//};

			//log.LogInformation(JsonConvert.SerializeObject(uploadDto));

			//return uploadDto;

			return new UploadDto();
		}
	}
}