using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Songs>>> GetSongs(int page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAuthenticated = !string.IsNullOrEmpty(userId);

            const int pageSize = 20;

            var songs = await (
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                select new
                {
                    s.Title,
                    s.SongId,
                    s.ImgUrl,
                    a.ArtistId,
                    ArtistName = a.Name,
                    s.ReleaseYear,
                    Noteringen = _context.Top2000Entries.Count(t => t.SongId == s.SongId),
                    IsLiked = isAuthenticated && _context.UserPlaylists.Any(up => up.UserId == userId && up.SongId == s.SongId)
                }
            )
            .Distinct()
            .OrderByDescending(x => x.Noteringen)
            .ThenBy(s => s.SongId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return Ok(songs);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Songs>> GetSong(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAuthenticated = !string.IsNullOrEmpty(userId);

            var song = await (
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where s.SongId == id
                orderby t.Year
                select new
                {
                    s.Youtube,
                    s.SongId,
                    s.Title,
                    a.ArtistId,
                    Artist = a.Name,
                    t.Position,
                    t.Year,
                    s.Lyrics,
                    s.ImgUrl,
                    s.ReleaseYear,
                    IsLiked = isAuthenticated && _context.UserPlaylists.Any(up => up.UserId == userId && up.SongId == s.SongId)
                }
            ).ToListAsync();

            return Ok(song);
        }

        [AllowAnonymous]
        [HttpGet("top5")]
        public async Task<ActionResult> GetTop5Songs()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAuthenticated = !string.IsNullOrEmpty(userId);

            var year = 2024;
            var previousYear = year - 1;

            var songs = await (
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where t.Year == year
                orderby t.Position
                let lastYearPosition = _context.Top2000Entries
                    .Where(py => py.SongId == s.SongId && py.Year == previousYear)
                    .Select(py => (int?)py.Position)
                    .FirstOrDefault()
                select new
                {
                    s.Title,
                    s.SongId,
                    s.ImgUrl,
                    a.ArtistId,
                    ArtistName = a.Name,
                    s.ReleaseYear,
                    Noteringen = _context.Top2000Entries.Count(te => te.SongId == s.SongId),
                    t.Position,
                    t.Year,

                    PositionDifference = lastYearPosition.HasValue
                        ? (lastYearPosition.Value - t.Position).ToString()
                        : "Nieuw",

                    IsLiked = isAuthenticated && _context.UserPlaylists.Any(up => up.UserId == userId && up.SongId == s.SongId)


                }).Take(5).ToListAsync();

            return Ok(songs);
        }

        [HttpGet("fulllist")]
        public async Task<ActionResult> GetFullList([FromQuery] int year, string? order)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAuthenticated = !string.IsNullOrEmpty(userId);

            if (year == 0) year = 2024;

            var previousYear = year - 1;

            var query =
                from t in _context.Top2000Entries
                join s in _context.Songs on t.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where t.Year == year
                let lastYearPosition = _context.Top2000Entries
                    .Where(py => py.SongId == s.SongId && py.Year == previousYear)
                    .Select(py => (int?)py.Position)
                    .FirstOrDefault()
                select new
                {
                    s.Title,
                    s.SongId,
                    s.ImgUrl,
                    a.ArtistId,
                    ArtistName = a.Name,
                    s.ReleaseYear,
                    Noteringen = _context.Top2000Entries.Count(te => te.SongId == s.SongId),
                    t.Position,
                    t.Year,

                    PositionDifference = lastYearPosition.HasValue
                        ? (lastYearPosition.Value - t.Position).ToString()
                        : "Nieuw",
                    IsLiked = isAuthenticated && _context.UserPlaylists.Any(up => up.UserId == userId && up.SongId == s.SongId)

                };

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
