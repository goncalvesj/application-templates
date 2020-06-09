using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NetCore.Durable.Functions.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.Durable.Functions.DurableFunctions
{
    public class RunOrchestrator
	{
		[FunctionName(nameof(RunOrchestrator))]
		public async Task<List<object>> Run(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var dto = context.GetInput<EventDto>();

			// QUERY REFERENCE DATA
			var mapResult = await context.CallActivityAsync<UploadDto>(nameof(GetDetailsActivity), dto);

			// UPLOAD
			var uploadResult = await context.CallActivityAsync<ModelResult>(nameof(FileUploadActivity), mapResult);

			// SAVE TO DB
			// await context.CallActivityAsync<string>(nameof(SayHelloActivity), "London")

			var outputs = new List<object>
			{
				dto,
				mapResult,
				uploadResult
			};

			return outputs;
		}
	}
}