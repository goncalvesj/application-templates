using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

// Disable warnings related to Semantic Kernel
#pragma warning disable SKEXP0003
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0011
#pragma warning disable SKEXP0021
#pragma warning disable SKEXP0052
#pragma warning disable SKEXP0060
#pragma warning disable SKEXP0061

// Azure OpenAI Variables
var apiKey = "";
var deploymentChatName = "";
var deploymentEmbeddingName = "";
var endpoint = "";

// Azure AI Search Variables
string searchApiKey = "";
string searchEndpoint = "";


var builder = Kernel.CreateBuilder();

builder
    .AddAzureOpenAIChatCompletion(
    deploymentChatName, 
    endpoint, 
    apiKey);

var kernel = builder.Build();

var embeddingConfig = new AzureOpenAIConfig
{
    APIKey = apiKey,
    Deployment = deploymentEmbeddingName,
    Endpoint = endpoint,
    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
    Auth = AzureOpenAIConfig.AuthTypes.APIKey
};

var chatConfig = new AzureOpenAIConfig
{
    APIKey = apiKey,
    Deployment = deploymentChatName,
    Endpoint = endpoint,
    APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
    Auth = AzureOpenAIConfig.AuthTypes.APIKey
};

var kernelMemory = new KernelMemoryBuilder()
    .WithAzureOpenAITextGeneration(chatConfig)
    .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
    .WithAzureAISearchMemoryDb(searchEndpoint, searchApiKey)
    .Build<MemoryServerless>();

// Import a document to the memory, e.g. Expense Policy PDF document
await kernelMemory.ImportDocumentAsync("CHANGE_ME: FILE_LOCATION.pdf", documentId: "doc001");

// Import the memory plugin
var plugin = new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true);
kernel.ImportPluginFromObject(plugin, "memory");

// Import the VCard plugin
kernel.ImportPluginFromType<VCardPlugin>();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("Hello! This is a chat console.");
Console.WriteLine(string.Empty);

var system = $@"
CHANGE_ME: WRITE YOUR ASSISTANT SYSTEM PROMPT HERE
";

history.AddMessage(AuthorRole.System, system);

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

while (true)
{
    Console.WriteLine(string.Empty);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Input > ");
    var message = Console.ReadLine();
    Console.ResetColor();

    // Prompt the message to the kernel memory
    var prompt = $@"
           Question to Kernel Memory: {message}
           Kernel Memory Answer: {{memory.ask}}
           ";

    history.AddMessage(AuthorRole.User, prompt);

    var result = await chatCompletionService.GetChatMessageContentAsync(history, openAIPromptExecutionSettings, kernel);

    Console.WriteLine(string.Empty);
    Console.WriteLine(string.Empty);
    Console.Write("Assistant > ");
    Console.WriteLine(result.Content);

    history.AddMessage(AuthorRole.Assistant, result.Content);
}

public class VCardPlugin
{
    const string FunctionModelDescription = @"
- This function creates a request for a Virtual Card.
- You must always ask for the user approval before creating a virtual card.
- You must always ask for the user policy details before creating a virtual card.
- You must always ask for the number of days of travel before creating a virtual card.
- You must always request the ammount to be created. 
- If the ammount is bigger than the allowed from the policy deny the request.
";

    [KernelFunction, Description(FunctionModelDescription)]
    public string CreateCard(int ammount, int days)
    {
        Random random = new Random();
        string cardNumber = "5" + random.Next(1, 6).ToString() + string.Concat(Enumerable.Range(0, 14).Select(n => random.Next(0, 10).ToString()));

        // Calculate the Luhn check digit
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            int digit = int.Parse(cardNumber[i].ToString());
            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        cardNumber += checkDigit.ToString();

        // Generate a random expiry date in the future
        int expiryMonth = random.Next(1, 13); // Month is between 1 and 12
        int expiryYear = DateTime.Now.Year + random.Next(1, 6); // Year is between now and 5 years from now

        // Generate a random 3-digit CVC
        int cvc = random.Next(100, 1000); // CVC is a 3-digit number

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Card created with ammount {ammount} for {days} days]");
        Console.WriteLine($"Card Number: {cardNumber}");
        Console.WriteLine($"Expiry Date: {expiryMonth:D2}/{expiryYear}");
        Console.WriteLine($"CVC: {cvc}");
        Console.ResetColor();

        return "Virtual Card created successfully.";
    }
}