using System.ComponentModel.DataAnnotations;

namespace TemplateJwtProject.Models
{
    public class Artists
    {
        [Key]
        public int ArtistId { get; set; }
        public string? Name { get; set; }
        public string? Wiki { get; set; }
        public string? Biography { get; set; }
        public string? Photo { get; set; }
    }
}
