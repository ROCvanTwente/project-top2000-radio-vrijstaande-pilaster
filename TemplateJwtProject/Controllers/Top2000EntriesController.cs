using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Top2000EntriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Top2000EntriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Top2000Entries>>> GetTop2000Entries()
        {
            var entries = await _context.Top2000Entries
                .Include(e => e.Song)
                .ThenInclude(s => s.Artist)
                .ToListAsync();

            return Ok(entries);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Top2000Entries>> GetTop2000Entry(int id)
        {
            var entry = await _context.Top2000Entries
                .Include(e => e.Song)
                .ThenInclude(s => s.Artist)
                .FirstOrDefaultAsync(e => e.SongId == id);

            if (entry == null)
                return NotFound();

            return Ok(entry);
        }

        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<Top2000Entries>>> GetByYear(int year)
        {
            var entries = await _context.Top2000Entries
                .Where(e => e.Year == year)
                .OrderBy(e => e.Position)
                .Include(e => e.Song)
                .ThenInclude(s => s.Artist)
                .ToListAsync();

            return Ok(entries);
        }


    }
}
