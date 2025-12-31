/*
 * LlmAgentService.cs
 * A reusable Azure OpenAI integration service for Blazor projects.
 * 
 * Usage:
 * 1. Copy this file to your project's Services/ folder.
 * 2. Register in Program.cs: builder.Services.AddScoped<LlmAgentService>();
 * 3. Configure appsettings.json with AzureOpenAI:Endpoint, ApiKey, DeploymentName.
 */

using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace TestPilot.Services;

public class LlmAgentService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<LlmAgentService> _logger;

    public LlmAgentService(IConfiguration config, ILogger<LlmAgentService> logger)
    {
        _logger = logger;

        var endpoint = config["AzureOpenAI:Endpoint"] 
            ?? throw new InvalidOperationException("Missing AzureOpenAI:Endpoint in configuration.");
        var apiKey = config["AzureOpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("Missing AzureOpenAI:ApiKey in configuration.");
        var deploymentName = config["AzureOpenAI:DeploymentName"] ?? "gpt-4o";

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _chatClient = client.GetChatClient(deploymentName);
    }

    /// <summary>
    /// Sends a simple chat completion request to the LLM.
    /// </summary>
    /// <param name="systemPrompt">The system prompt defining the AI's role.</param>
    /// <param name="userMessage">The user's input message.</param>
    /// <returns>The LLM's response text.</returns>
    public async Task<string> ChatAsync(string systemPrompt, string userMessage)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM chat request failed.");
            throw;
        }
    }

    /// <summary>
    /// Sends a chat completion request with an image (Vision) to the LLM.
    /// </summary>
    /// <param name="systemPrompt">The system prompt.</param>
    /// <param name="userMessage">The text instructions accompanying the image.</param>
    /// <param name="base64Image">The base64 encoded image string.</param>
    /// <param name="mediaType">The MIME type of the image (e.g., "image/png").</param>
    /// <returns>The LLM's analysis.</returns>
    public async Task<string> ChatImageAsync(string systemPrompt, string userMessage, string base64Image, string mediaType = "image/png")
    {
        try
        {
            // Create image content part
            var imageContent = ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes(Convert.FromBase64String(base64Image)), mediaType);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(userMessage),
                    imageContent
                )
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM vision request failed.");
            throw;
        }
    }

    /// <summary>
    /// Sends a chat completion request and expects a JSON response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON response into.</typeparam>
    /// <param name="systemPrompt">The system prompt.</param>
    /// <param name="userMessage">The user's input.</param>
    /// <returns>The deserialized response object.</returns>
    public async Task<T?> ChatJsonAsync<T>(string systemPrompt, string userMessage)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            };

            var options = new ChatCompletionOptions 
            { 
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() 
            };

            var response = await _chatClient.CompleteChatAsync(messages, options);
            var json = response.Value.Content[0].Text;

            var serializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            serializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

            return System.Text.Json.JsonSerializer.Deserialize<T>(json, serializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM JSON chat request failed.");
            throw;
        }
    }
}
