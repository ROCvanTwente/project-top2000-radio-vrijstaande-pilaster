using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemplateJwtProject.Models
{
    public class Songs
    {
        [Key]
        public int SongId { get; set; }
        public int ArtistId { get; set; }

        public string? Title { get; set; }
        public int ReleaseYear { get; set; }
        public string? ImgUrl { get; set; }
        public string? Lyrics { get; set; }
        public string? Youtube { get; set; }

        // Navigation property
        public Artists? Artist { get; set; }

    }
}
