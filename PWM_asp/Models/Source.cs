using System.ComponentModel.DataAnnotations;

namespace PWM_asp.Models
{
    public class Source
    {
        [Key]
        public int SourceId {  get; set; }
        [Required]
        public string SourceName { get; set; } = string.Empty;
        public string? Description {  get; set; } = string.Empty;
        public virtual ICollection<SavedPWD> SavedPWD { get; set; } = new List<SavedPWD>();
        public virtual ICollection<ArchivedPWD> ArchivedPWD { get; set; } = new List<ArchivedPWD>(); 
    }
}
