using System.ComponentModel.DataAnnotations;

public class SavedPWDViewModel
{
    public int SavedPWDId { get; set; }

    public string Account { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
