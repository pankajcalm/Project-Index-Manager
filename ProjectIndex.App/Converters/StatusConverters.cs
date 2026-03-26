using System.Globalization;

namespace ProjectIndex.App.Converters;

public class StatusBgConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value?.ToString() switch
        {
            "Active" => Color.FromArgb("#EAF3DE"),
            "Paused" => Color.FromArgb("#FAEEDA"),
            "Idea" => Color.FromArgb("#EEEDFE"),
            "Archived" => Color.FromArgb("#F1EFE8"),
            _ => Colors.Transparent
        };

    public object ConvertBack(object? v, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

public class StatusFgConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c) =>
        value?.ToString() switch
        {
            "Active" => Color.FromArgb("#3B6D11"),
            "Paused" => Color.FromArgb("#854F0B"),
            "Idea" => Color.FromArgb("#534AB7"),
            "Archived" => Color.FromArgb("#5F5E5A"),
            _ => Colors.Gray
        };

    public object ConvertBack(object? v, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => !string.IsNullOrWhiteSpace(value?.ToString());

    public object ConvertBack(object? v, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}
