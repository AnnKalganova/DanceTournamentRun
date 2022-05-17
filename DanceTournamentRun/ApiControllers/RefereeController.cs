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
    [Route("api/[controller]/{token}")]
    [ApiController]
    public class RefereeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RefereeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Referee/{token}
        //get user name
        [HttpGet]
        public ActionResult<string> GetUserInfo(string token)
        {
            User user = _context.Users.FirstOrDefault(u => u.SecurityToken == token);
            if (user != null)
            {
                return user.LastName + " " + user.FirstName;
            }
            return NotFound();
        }


        //GET: api/Referee/{token}/groups
        //get groups by token
        [HttpGet("groups")]
        public ActionResult<List<Group>> GetGroups(string token)
        {

            List<Group> groups = new List<Group>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groups = db.GetGroupsByToken(token);
            }
            if (groups.Count() == 0)
                return NotFound();
            return groups;
        }

    }
}
