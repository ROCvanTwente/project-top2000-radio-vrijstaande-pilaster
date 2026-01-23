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
            var song = await (
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where s.SongId == id
                orderby t.Year
                select new
                {
                    s.Title,
                    a.ArtistId,
                    Artist = a.Name,
                    t.Position,
                    t.Year,
                    s.Lyrics,
                    s.ImgUrl

                }
            ).ToListAsync();

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
                    s.SongId,
                    a.ArtistId,
                    t.Position,
                    s.Title,
                    Artist = a.Name
                }
            ).ToListAsync();

            return Ok(top5);
        }

        [HttpGet("fulllist")]
        public async Task<ActionResult> GetFullList([FromQuery] int year, string? order)
        {
            // Optional: set a default if no year is provided
            if (year == 0) year = 2024;

            Console.WriteLine(order);

            var query = from t in _context.Top2000Entries
                        join s in _context.Songs on t.SongId equals s.SongId
                        join a in _context.Artists on s.ArtistId equals a.ArtistId
                        where t.Year == year
                        select new
                        {
                            t.Year,
                            s.SongId,
                            t.Position,
                            s.Title,
                            Artist = a.Name,
                            s.ArtistId
                        };

            // Apply ordering based on the order parameter
            if (!string.IsNullOrEmpty(order))
            {
                if (order.Equals("ASC", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(x => x.Title);
                }
                else if (order.Equals("DESC", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(x => x.Title);
                }
                else
                {
                    query = query.OrderBy(x => x.Position);
                }
            }
            else
            {
                query = query.OrderBy(x => x.Position);
            }

            var list = await query.ToListAsync();

            return Ok(list);
        }
    }
}
