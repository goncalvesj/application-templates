using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NetCore.Durable.Functions.Dto;
using NetCore.Durable.Functions.Validators;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Durable.Functions.DurableFunctions
{
    public static class StartOrchestrator
	{
		[FunctionName(nameof(StartOrchestrator))]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, methods: "post", Route = null)] EventDto eventDto,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
            var validator = new EventDtoValidator();
            var validationResult = await validator.ValidateAsync(eventDto);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(
                    validationResult.Errors.Select(e => new ModelErrorResult(e.PropertyName, e.ErrorMessage)));
            }

			var instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), eventDto);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

			var status = await starter.GetStatusAsync(instanceId);

			while (status.RuntimeStatus != OrchestrationRuntimeStatus.Completed)
			{
				await Task.Delay(1000);
				status = await starter.GetStatusAsync(instanceId);

				if ((status.RuntimeStatus == OrchestrationRuntimeStatus.Failed)
					|| (status.RuntimeStatus == OrchestrationRuntimeStatus.Terminated)
					|| (status.RuntimeStatus == OrchestrationRuntimeStatus.Canceled))
				{
					throw new FunctionFailedException("Orchestration failed with error: " + status.Output);
				}
			}

			return new ObjectResult(status.Output) { StatusCode = StatusCodes.Status201Created };
		}
	}
}