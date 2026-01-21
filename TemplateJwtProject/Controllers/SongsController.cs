using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly Data.AppDbContext _context;
        public SongsController(Data.AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Songs>>> GetSongs()
        {
            var songs = await _context.Songs.ToListAsync();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Songs>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }


        [HttpGet("top5")]
        public async Task<ActionResult> GetTop5Songs()
        {
            var top5 = await (
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where t.Year == 2024 && t.Position >= 1 && t.Position <= 5
                orderby t.Position
                select new
                {
                    t.Position,
                    s.Title,
                    Artist = a.Name
                }
            ).ToListAsync();

            return Ok(top5);
        }


    }
}
