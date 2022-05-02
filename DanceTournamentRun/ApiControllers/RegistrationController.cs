using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [EnableCors("AllowMyOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Registration/groupsByDept/3
        // get groups by department's id
        //[HttpGet("groupsByDept/{deptId}")]
        //public async Task<ActionResult<IEnumerable<Group>>> GetGroupsByDepartment(long deptId)
        //{
        //    var groups = await _context.Groups.Where(x => x.DepartmentId == deptId).ToListAsync();
        //    if (groups.Count() == 0)
        //        return NotFound();
        //    return groups;
        //}

        // GET: api/Registration/pairsByGroup/5
        //get pairs by group's id 
        [HttpGet("pairsByGroup/{grId}")]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairsByGroup(long grId)
        {
            var pairs = await _context.Pairs.Where(x => x.GroupId == grId).ToListAsync();

            if (pairs.Count() == 0)
                return NotFound();
            return pairs;
        }

        // GET: api/Registration/2
        [HttpGet("pair/{Id}")]
        public async Task<ActionResult<Pair>> GetPair(long Id)
        {
            var pair = await _context.Pairs.FindAsync(Id);
            if (pair == null)
            {
                return NotFound();
            }
            return pair;
        }

        // POST: api/Registration
        [HttpPost]
        public async Task<ActionResult<Pair>> CreatePair(Pair pair)
        {
            _context.Pairs.Add(pair);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPair", new { id = pair.Id }, pair);
        }

        // PUT: api/Registration/5/3
        [HttpPut("{id}/{number}")]
        public async Task<IActionResult> EditPairInfo(long id, int number)
        {
            var pair = await _context.Pairs.FindAsync(id);
            if (pair == null)
            {
                return NotFound();
            }
            pair.Number = number;
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
                else { throw; }
            }
            return NoContent();
        }

        // PUT: api/Registration/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPairInfo(long id, Pair pair)
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
                else { throw; }
            }
            return NoContent();
        }

        // DELETE: api/Registration/5
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