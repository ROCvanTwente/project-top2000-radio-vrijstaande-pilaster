using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Constants;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditController : ControllerBase
    {
        private readonly Data.AppDbContext _context;
        public EditController(Data.AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("song")]
        public async Task<ActionResult> EditSong([FromBody] Songs model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var song = await _context.Songs.FindAsync(model.SongId);

            if (song == null)
            {
                return NotFound();
            }

            song!.Title = model.Title;
            song.ArtistId = model.ArtistId;
            song.ReleaseYear = model.ReleaseYear;
            song.ImgUrl = model.ImgUrl;
            song.Lyrics = model.Lyrics; 
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("artist")]
        public async Task<ActionResult> EditArtist([FromBody] Artists model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var artist = await _context.Artists.FindAsync(model.ArtistId);

            if (artist == null)
            {
                return NotFound();
            }

            artist!.Name = model.Name;
            artist.Biography = model.Biography;
            artist.Photo = model.Photo;
            artist.Wiki = model.Wiki;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
