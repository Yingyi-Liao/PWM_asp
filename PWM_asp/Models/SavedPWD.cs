using System.ComponentModel.DataAnnotations;

namespace PWM_asp.Models
{
    public class SavedPWD
    {
        [Key]
        public int SavedPWDId { get; set; }
        [Required]
        public string Account {  get; set; } = string.Empty;
        [Required]
        public string EncryptedPWD { get; set; } = string.Empty;
        public string EncryptedDataKey { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual AppUser User { get; set; } = null!;
        public virtual Source Source { get; set; } = null!;

    }
}
