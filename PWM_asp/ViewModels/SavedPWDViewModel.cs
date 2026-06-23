using System.ComponentModel.DataAnnotations;

public class SavedPWDViewModel
{
    public int SavedPWDId { get; set; }

    [Required]
    public string Account { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string SourceName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
