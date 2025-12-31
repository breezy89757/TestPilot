using Microsoft.Playwright;

namespace TestPilot.Services;

public interface IPlaywrightService
{
    Task<string> RunTestAsync(string url, string browserType);
}

public class PlaywrightService : IPlaywrightService
{
    public async Task<string> RunTestAsync(string url, string browserType)
    {
        using var playwright = await Playwright.CreateAsync();
        
        IBrowser browser;
        var launchOptions = new BrowserTypeLaunchOptions { Headless = true };

        switch (browserType.ToLower())
        {
            case "firefox":
                browser = await playwright.Firefox.LaunchAsync(launchOptions);
                break;
            case "webkit":
                browser = await playwright.Webkit.LaunchAsync(launchOptions);
                break;
            default:
                browser = await playwright.Chromium.LaunchAsync(launchOptions);
                break;
        }

        try
        {
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            
            var title = await page.TitleAsync();
            
            // Capture screenshot
            var screenshotBytes = await page.ScreenshotAsync();
            var base64Screenshot = Convert.ToBase64String(screenshotBytes);
            
            return base64Screenshot;
        }
        finally
        {
            await browser.CloseAsync();
        }
    }
}
