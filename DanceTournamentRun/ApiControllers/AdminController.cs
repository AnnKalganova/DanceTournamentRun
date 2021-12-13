using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pairs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pair>>> ShowRefereeLink()
        {
            //generateRefereeUrl()
            //generateRefereeQR()
            return await _context.Pairs.ToListAsync();
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> ShowRegistrationLink()
        //{
        //    //generateRegistrationUrl()
        //    //generateRegistrationQR()
        //    return await _context.Pairs.ToListAsync();
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> GetPairsWithNumbers()
        //{
        //    //check if registraion is finished
        //    // show all pairs by groups
        //    return await _context.Pairs.ToListAsync();
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> GetRefereeingResults()
        //{
        //    return await _context.Pairs.ToListAsync();
        //}
    }
}
