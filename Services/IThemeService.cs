namespace JournalApp.Services;

public interface IThemeService
{
    Task InitializeTheme();
    Task ToggleTheme();
    Task<bool> IsDarkMode();
}
