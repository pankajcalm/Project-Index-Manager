namespace ProjectIndex.App.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.UnhandledException += (sender, e) =>
        {
            var logPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "projectindex_crash.log");
            System.IO.File.WriteAllText(logPath,
                $"{DateTime.Now}\n{e.Exception}\n{e.Exception.StackTrace}");
            e.Handled = true;
        };
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
