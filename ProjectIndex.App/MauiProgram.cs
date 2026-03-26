using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using ProjectIndex.App.Pages;
using ProjectIndex.Core.Data;
using ProjectIndex.Core.Services;

namespace ProjectIndex.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory, "projects.db");

        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<ProjectService>();

        // Pages
        builder.Services.AddTransient<ProjectListPage>();
        builder.Services.AddTransient<AddEditProjectPage>();

        return builder.Build();
    }
}
