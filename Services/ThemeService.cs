using Microsoft.JSInterop;
using Microsoft.Maui.Storage; // Explicitly add this

namespace JournalApp.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private const string ThemeKey = "app_theme";

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeTheme()
    {
        var theme = Preferences.Get(ThemeKey, "light");
        Console.WriteLine($"[ThemeService] Initializing theme to: {theme}");
        await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
    }

    public async Task ToggleTheme()
    {
        var currentTheme = Preferences.Get(ThemeKey, "light");
        var newTheme = currentTheme == "light" ? "dark" : "light";
        
        Console.WriteLine($"[ThemeService] Toggling theme from {currentTheme} to {newTheme}");
        
        Preferences.Set(ThemeKey, newTheme);
        await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", newTheme);
    }

    public Task<bool> IsDarkMode()
    {
        var theme = Preferences.Get(ThemeKey, "light");
        return Task.FromResult(theme == "dark");
    }
}
