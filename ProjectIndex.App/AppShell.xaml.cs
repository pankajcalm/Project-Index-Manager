using ProjectIndex.App.Pages;

namespace ProjectIndex.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(AddEditProjectPage), typeof(AddEditProjectPage));
    }
}
