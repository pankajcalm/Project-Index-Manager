using ProjectIndex.Core.Models;
using ProjectIndex.Core.Services;

namespace ProjectIndex.App.Pages;

public partial class ProjectListPage : ContentPage
{
    private readonly ProjectService _service;
    private List<Project> _allProjects = [];

    public ProjectListPage(ProjectService service)
    {
        InitializeComponent();
        _service = service;

        StatusPicker.ItemsSource = new List<string>
            { "All", "Active", "Paused", "Idea", "Archived" };
        StatusPicker.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProjectsAsync();
    }

    private async Task LoadProjectsAsync()
    {
        _allProjects = await _service.SearchAsync();

        // Stats
        StatTotal.Text = _allProjects.Count.ToString();
        StatActive.Text = _allProjects.Count(p => p.Status == "Active").ToString();
        StatPaused.Text = _allProjects.Count(p => p.Status == "Paused").ToString();
        StatIdea.Text = _allProjects.Count(p => p.Status == "Idea").ToString();
        SubtitleLabel.Text = $"{_allProjects.Count} projects tracked";

        // Rebuild category picker
        var cats = _allProjects
            .Where(p => !string.IsNullOrWhiteSpace(p.Category))
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .Prepend("All")
            .ToList();

        CategoryPicker.ItemsSource = cats;
        CategoryPicker.SelectedIndex = 0;

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var query = SearchBar.Text?.Trim().ToLower() ?? string.Empty;
        var status = StatusPicker.SelectedItem?.ToString();
        var category = CategoryPicker.SelectedItem?.ToString();

        var filtered = _allProjects.AsEnumerable();

        if (!string.IsNullOrEmpty(query))
            filtered = filtered.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (p.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.Tags?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.Path?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));

        if (status is not null && status != "All")
            filtered = filtered.Where(p => p.Status == status);

        if (category is not null && category != "All")
            filtered = filtered.Where(p => p.Category == category);

        ProjectsCollection.ItemsSource = filtered
            .OrderByDescending(p => p.UpdatedAt)
            .ToList();
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
    private void OnFilterChanged(object? sender, EventArgs e) => ApplyFilters();

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadProjectsAsync();
        RefreshView.IsRefreshing = false;
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddEditProjectPage));
    }

    private async void OnProjectTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Project p)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(AddEditProjectPage)}?id={p.Id}");
        }
    }

    private async void OnEditSwiped(object? sender, EventArgs e)
    {
        if (sender is SwipeItem { CommandParameter: Project p })
        {
            await Shell.Current.GoToAsync(
                $"{nameof(AddEditProjectPage)}?id={p.Id}");
        }
    }

    private async void OnDeleteSwiped(object? sender, EventArgs e)
    {
        if (sender is SwipeItem { CommandParameter: Project p })
        {
            bool confirm = await DisplayAlert(
                "Delete", $"Remove '{p.Name}'?", "Delete", "Cancel");

            if (confirm)
            {
                await _service.DeleteAsync(p.Id);
                await LoadProjectsAsync();
            }
        }
    }
}
