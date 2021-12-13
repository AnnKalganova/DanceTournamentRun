using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DanceTournamentRun;
using DanceTournamentRun.Models;

namespace DanceTournamentRun.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PairsController : ControllerBase
    {
        private  ApplicationDbContext _context;

        public PairsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pairs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairs()
        {
            return await _context.Pairs.ToListAsync();
        }

        // GET: api/Pairs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pair>> GetPair(long id)
        {
            var pair = await _context.Pairs.FindAsync(id);

            if (pair == null)
            {
                return NotFound();
            }

            return pair;
        }

        // PUT: api/Pairs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPair(long id, Pair pair)
        {
            if (id != pair.Id)
            {
                return BadRequest();
            }

            _context.Entry(pair).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PairExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pairs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pair>> PostPair(Pair pair)
        {
            _context.Pairs.Add(pair);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPair", new { id = pair.Id }, pair);
        }

        // DELETE: api/Pairs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePair(long id)
        {
            var pair = await _context.Pairs.FindAsync(id);
            if (pair == null)
            {
                return NotFound();
            }

            _context.Pairs.Remove(pair);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PairExists(long id)
        {
            return _context.Pairs.Any(e => e.Id == id);
        }
    }
}
