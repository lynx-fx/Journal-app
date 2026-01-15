using Microsoft.Extensions.Logging;

namespace journal_app;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<JournalApp.Data.AppDatabase>();
        builder.Services.AddSingleton<JournalApp.Services.IJournalsServices, JournalApp.Services.JournalsServices>();
		builder.Services.AddSingleton<JournalApp.Services.ITagsServices, JournalApp.Services.TagsServices>();
		builder.Services.AddSingleton<JournalApp.Services.IJournalTagsServices, JournalApp.Services.JournalTagsServices>();
		builder.Services.AddScoped<JournalApp.Services.IThemeService, JournalApp.Services.ThemeService>();
        builder.Services.AddScoped<JournalApp.Services.IUsersService, JournalApp.Services.UsersService>();
        builder.Services.AddScoped<JournalApp.Services.IMoodsService, JournalApp.Services.MoodsService>();
        builder.Services.AddScoped<JournalApp.Services.IAnalyticsService, JournalApp.Services.AnalyticsService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
