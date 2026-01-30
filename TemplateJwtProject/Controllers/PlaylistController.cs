using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TemplateJwtProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaylistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut]
        public async Task<IActionResult> AddToPlaylist([FromBody] PlaylistRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var exists = await _context.UserPlaylists
                .AnyAsync(up => up.UserId == userId && up.SongId == request.SongId);

            if (exists)
                return BadRequest("Nummer staat al in playlist!");

            _context.UserPlaylists.Add(new UserPlaylists
            {
                UserId = userId,
                SongId = request.SongId
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromPlaylist([FromBody] PlaylistRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var entry = await _context.UserPlaylists
                .FirstOrDefaultAsync(up => up.UserId == userId && up.SongId == request.SongId);

            if (entry == null)
                return NotFound();

            _context.UserPlaylists.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ShowPlaylist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var playlist = await (
                from up in _context.UserPlaylists
                join s in _context.Songs on up.SongId equals s.SongId
                join a in _context.Artists on s.ArtistId equals a.ArtistId
                where up.UserId == userId
                select new
                {
                    s.SongId,
                    s.Title,
                    s.ImgUrl,
                    a.ArtistId,
                    ArtistName = a.Name,
                    s.ReleaseYear,
                    IsLiked = true
                }
            )
            .ToListAsync();

            return Ok(playlist);
        }


    }

    public class PlaylistRequest
    {
        public int SongId { get; set; }
    }

}
