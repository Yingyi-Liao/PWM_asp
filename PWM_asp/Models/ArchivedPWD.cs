using System.ComponentModel.DataAnnotations;

namespace PWM_asp.Models
{
    public class ArchivedPWD
    {
        [Key]
        public int ArchivedPWDId { get; set; }
        [Required]
        public string Account { get; set; } = string.Empty;
        [Required]
        public string EncryptedPWD { get; set; } = string.Empty;
        public string EncryptedDataKey { get; set; } = string.Empty;
        public int SourceId { get; set; }
        public string? Description { get; set; }
        public DateTime ArchivedAt { get; set; }
        public string ArchivedBy { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public virtual AppUser User { get; set; } = null!;
        public virtual Source Source { get; set; } = null!;
    }
}
