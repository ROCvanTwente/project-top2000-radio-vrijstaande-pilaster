using System.ComponentModel.DataAnnotations;

namespace TemplateJwtProject.Models
{
    public class Top2000Entries
    {
        public int SongId { get; set; }
        public Songs? Song { get; set; }

        public int Year { get; set; }
        public int Position { get; set; }
    }
}
