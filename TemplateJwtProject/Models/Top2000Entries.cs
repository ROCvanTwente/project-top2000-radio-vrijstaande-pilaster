using System.ComponentModel.DataAnnotations;
using TemplateJwtProject.Models;

public class Top2000Entries
{
    [Key]
    public int Top2000EntryId { get; set; }

    public int SongId { get; set; }
    public Songs? Song { get; set; }

    public int Year { get; set; }
    public int Position { get; set; }
}
