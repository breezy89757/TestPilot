using TestPilot.Services;

namespace TestPilot.Services;

public class AiAnalysisService
{
    private readonly LlmAgentService _llmService;
    private readonly ILogger<AiAnalysisService> _logger;

    public AiAnalysisService(LlmAgentService llmService, ILogger<AiAnalysisService> logger)
    {
        _llmService = llmService;
        _logger = logger;
    }

    public async Task<string> AnalyzeScreenshotAsync(string base64Image)
    {
        var systemPrompt = @"You are an expert QA Automation Engineer and UI/UX Designer.
Your task is to analyze the provided screenshot of a web application.
1. Identify the main elements visible on the page.
2. Check for any obvious visual defects (broken layout, overlapping text, missing images).
3. Evaluate the UI/UX quality based on modern design standards.
4. Provide a summary of what this page appears to be and if it looks 'correct'.
Keep your response concise but professional.";

        var userMessage = "Analyze this screenshot and tell me if the verification test passed visual inspection.";

        try
        {
            return await _llmService.ChatImageAsync(systemPrompt, userMessage, base64Image, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Analysis failed.");
            return $"AI Analysis Failed: {ex.Message}";
        }
    }
}
