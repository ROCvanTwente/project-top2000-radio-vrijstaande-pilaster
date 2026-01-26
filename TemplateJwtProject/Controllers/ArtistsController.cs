using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArtistsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artists>>> GetArtists(int page = 1)
        {
            const int pageSize = 20;

            var artists = await (

                from a in _context.Artists
                
                select new {
                    a.ArtistId,
                    a.Photo,
                    a.Name,
                    Noteringen = _context.Songs.Count(s => s.ArtistId == a.ArtistId)
                }

            )
            .OrderByDescending(x => x.Noteringen)
            .ThenBy(x => x.ArtistId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return Ok(artists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Artists>> GetArtist(int id)
        {
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
                return NotFound();

            return Ok(artist);
        }



    }


}
