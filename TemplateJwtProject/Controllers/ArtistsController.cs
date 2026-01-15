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
        public async Task<ActionResult<IEnumerable<Artists>>> GetArtists()
        {
            var artists = await _context.Artists.ToListAsync();
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
