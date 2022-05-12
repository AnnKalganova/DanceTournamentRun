using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.Controllers
{
    [Authorize(Roles = "admin")]
    public class RunTournamentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RunTournamentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                return RedirectToAction("CreateTournament");
            }
            if (!(bool)tournament.IsTournamentRun)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Login = _context.Users.Where(u => u.Id == tournament.UserId).Select(p => p.Login).FirstOrDefault();
            return View(tournament);
        }

        public ActionResult GetData(long? tournId)
        {
            if (tournId != null)
            {
                var tourn = _context.Tournaments.Find(tournId);
                if (!(bool)tourn.IsFirstStepOver)
                {
                    return RedirectToAction("GetFirstView");
                }
                else if (!(bool)tourn.IsSecondStepOver)
                {
                    return RedirectToAction("GetSecondView");
                }
                else
                {
                    return RedirectToAction("GetThirdView");
                }

            }
            return PartialView("Error");
        }

        public PartialViewResult GetFirstView(Tournament tournament)
        {
            if(tournament != null)
            {
                return PartialView("FirstStep");
            }
            return PartialView("Error");
        }

        public PartialViewResult GetSecondView(Tournament tournament)
        {
            if (tournament != null)
            {
                return PartialView("SecondStep");
            }
            return PartialView("Error");
        }

        public PartialViewResult GetThirdView(Tournament tournament)
        {
            if (tournament != null)
            {
                return PartialView("EndStep");
            }
            return PartialView("Error");
        }
    }
}


       
