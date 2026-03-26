using CommunityToolkit.Maui.Storage;
using ProjectIndex.Core.Models;
using ProjectIndex.Core.Services;

namespace ProjectIndex.App.Pages;

[QueryProperty(nameof(ProjectId), "id")]
public partial class AddEditProjectPage : ContentPage
{
    private readonly ProjectService _service;
    private Project? _project;
    private bool _bypassPathCheck;

    public string PageTitle => _project?.Id > 0 ? "Edit Project" : "Add Project";

    private int _projectId;
    public string ProjectId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _projectId = id;
        }
    }

    public AddEditProjectPage(ProjectService service)
    {
        InitializeComponent();
        _service = service;
        BindingContext = this;

        StatusPicker.ItemsSource = new List<string>
            { "Active", "Paused", "Idea", "Archived" };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (_projectId > 0)
        {
            _project = await _service.GetByIdAsync(_projectId);
            if (_project is null)
            {
                await DisplayAlert("Error", "Project not found.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }
            PopulateFields(_project);
        }
        else
        {
            _project = new Project();
            StatusPicker.SelectedIndex = 0; // default: Active
        }

        OnPropertyChanged(nameof(PageTitle));
    }

    private void PopulateFields(Project p)
    {
        NameEntry.Text = p.Name;
        DescriptionEditor.Text = p.Description;
        StatusPicker.SelectedItem = p.Status;
        CategoryEntry.Text = p.Category;
        TagsEntry.Text = p.Tags;
        PathEntry.Text = p.Path;
        RepoEntry.Text = p.RepoUrl;
        NotesEditor.Text = p.Notes;
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            ShowValidation("Project name is required.");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(PathEntry.Text) &&
            !PathEntry.Text.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !Directory.Exists(PathEntry.Text) &&
            !File.Exists(PathEntry.Text))
        {
            if (_bypassPathCheck)
            {
                _bypassPathCheck = false;
                HideValidation();
                return true;
            }

            ShowValidation("Path does not exist on this machine. Press Save again to confirm.");
            _bypassPathCheck = true;
            return false;
        }

        _bypassPathCheck = false;
        HideValidation();
        return true;
    }

    private void ShowValidation(string msg)
    {
        ValidationLabel.Text = msg;
        ValidationLabel.IsVisible = true;
    }

    private void HideValidation()
    {
        ValidationLabel.Text = string.Empty;
        ValidationLabel.IsVisible = false;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (!Validate()) return;

        _project!.Name = NameEntry.Text!.Trim();
        _project.Description = DescriptionEditor.Text?.Trim();
        _project.Status = StatusPicker.SelectedItem?.ToString() ?? "Active";
        _project.Category = CategoryEntry.Text?.Trim();
        _project.Tags = TagsEntry.Text?.Trim();
        _project.Path = PathEntry.Text?.Trim();
        _project.RepoUrl = RepoEntry.Text?.Trim();
        _project.Notes = NotesEditor.Text?.Trim();

        await _service.SaveAsync(_project);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        if (HasUnsavedChanges())
        {
            bool discard = await DisplayAlert(
                "Discard changes?", "You have unsaved changes.", "Discard", "Keep editing");
            if (!discard) return;
        }
        await Shell.Current.GoToAsync("..");
    }

    private async void OnBrowseClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
                PathEntry.Text = result.Folder.Path;
        }
        catch
        {
            await DisplayAlert("Not supported",
                "Folder picker is not available on this platform.", "OK");
        }
    }

    private bool HasUnsavedChanges() =>
        NameEntry.Text != (_project?.Name ?? string.Empty) ||
        DescriptionEditor.Text != (_project?.Description ?? string.Empty) ||
        PathEntry.Text != (_project?.Path ?? string.Empty);
}
