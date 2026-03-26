using ProjectIndex.Core.Data;

namespace ProjectIndex.App;

public partial class App : Application
{
    public App(AppDbContext db)
    {
        InitializeComponent();

        try
        {
            db.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DB init error: {ex.Message}");
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell())
        {
            Title = "Project Index",
            Width = 900,
            Height = 700,
            MinimumWidth = 600,
            MinimumHeight = 500
        };
    }
}
