﻿using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [EnableCors("AllowMyOrigin")]
    [Route("api/[controller]/{token}")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Registration/{token}
        //get user name
        [HttpGet]
        public ActionResult<UserInfoModel> GetUserInfo(string token)
        {
            User user = _context.Users.FirstOrDefault(u => u.SecurityToken == token);
            if (user != null)
            {
                return new UserInfoModel() { LastName = user.LastName, FirstName = user.FirstName};
            }
            return NotFound();
        }

        //GET: api/Registration/{token}/groups
        //get groups by token
        [HttpGet("groups")]
        public ActionResult<List<Group>> GetGroups(string token) {

            List<Group> groups = new List<Group>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groups = db.GetGroupsByToken(token);
            }
            if (groups.Count() == 0)
                return NotFound();
            return groups;
        }

        //GET: api/Registration/{token}/pairs/{grId}
        //get pairs by group with token verification
        [HttpGet("pairs/{grId}")]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairsByGroup(string token, long grId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(grId, token);
            }
            if (access != 0)
            {
                var pairs = await _context.Pairs.Where(x => x.GroupId == grId).ToListAsync();
                return pairs;
            }
            return NotFound();
        }

        //POST: api/Registration/{token}/updatePair/{pair}
        //update pair by pair object with token verification
        [HttpPost("updatePair")]
        public async Task<IActionResult> UpdatePair(string token,[FromBody] Pair pair)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(pair.GroupId, token);
            }
            //TODO проверка на то что такая пара есть или номер есть
            var oldPair = _context.Pairs.FirstOrDefault(p => p.Id == pair.Id);
            if (oldPair == null || access == 0)
            {
                return NotFound();
            }
            oldPair = pair;
            await _context.SaveChangesAsync();
            return Ok();
        }

        //POST:api/Registration/{token}/createPair
        // create pair by pair object from body with token verification
        [HttpPost("createPair")]
        public async Task<IActionResult> CreatePair(string token,[FromBody] CreatePairModel pair)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(pair.GroupId, token);
            }
            if (access == 0)
            {
                return NotFound();
            }
            //TODO проверка на то что такая пара\номер уже есть
            Pair newPair = new Pair()
            {
                GroupId = pair.GroupId,
                Partner1LastName = pair.Partner1LastName,
                Partner1FirstName = pair.Partner1FirstName,
                Partner2LastName = pair.Partner2LastName,
                Partner2FirstName = pair.Partner2FirstName,
                Number = pair.Number
            };
            _context.Pairs.Add(newPair);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Registration/{token}/deletePair/{pairId}/{groupId}
        //delete pair by id with token verification
        [HttpPost("deletePair/{pairId}/{groupId}")]
        public async Task<IActionResult> DeletePair(string token, long pairId, long groupId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(groupId, token);
            }
            var pair = _context.Pairs.Find(pairId);
            if (pair == null || access == 0)
            {
                return NotFound();
            }
            _context.Pairs.Remove(pair);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("complete")]
        public async Task<ActionResult> CompleteRegistration(string token)
        {
            //List<Group> groups = new List<Group>();
            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    groups = db.GetGroupsByToken(token);
            //}
            //if (groups.Count() == 0)
            //    return NotFound();
            return RedirectToAction("UpdateRegProgress", "RunTournament");
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

    }
}