using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            Console.WriteLine(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                return RedirectToAction("CreateTournament");
            }
            if ((bool)tournament.IsTournamentRun)
            {
                //redirect to tournament run 

            }

            ViewBag.TournName = tournament.Name;
            return View("SetupTournament",tournament);


            //if (TempData["RefLastname"] != null)
            //{
            //    ViewBag.RefLastname = TempData["RefLastname"];
            //}

        }

        [HttpGet]
        public ActionResult CreateTournament()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTournament(CreateTournModel model)
        {
            if (ModelState.IsValid)
            {
                long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Tournament tournament = new Tournament { Name = model.Name, UserId = userId, IsTournamentRun = false };
                _context.Tournaments.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            model.Name = "Ввведите название";
            return View(model);
        }

        
        public PartialViewResult ViewGroups(long? tournId)
        {
            //TODO: заменить на обращение к запросам внутри бд
            if(tournId != null)
            {
                var groups = _context.Groups.Where(p => p.TournamentId == tournId);
                ViewBag.tournamentId = tournId;
                ViewBag.groups = groups;
                return PartialView("Groups");
            }
            ViewBag.message = "Id турнира - " + tournId;
            //TODO: исправить на вывод ошибки
            return PartialView("Error");
        }

        [HttpPost]
        public async Task<ActionResult> AddGroup(long? tournId, string Name)
        {
            if (Name != null && tournId != null)
            {
                var groups = _context.Groups.Where(g => g.TournamentId == tournId).Select(x => x.Number);
                var newNumber = groups.Max() + 1;
                Group group = new Group { Name = Name, Number = newNumber, TournamentId = (long)tournId };
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewGroups", new { tournId = tournId });
            }
            return NotFound();
        }

        public ActionResult GetRegLinks()
        {
            string message = null;
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync($"{"https://localhost:44362/api/Admin/regLinks" }").Result;
                message = response.Content.ReadAsStringAsync().Result;
                var result  = JsonConvert.DeserializeObject<RegLinksViewModel>(message);
                Console.WriteLine(result);
                TempData["RegGroupsId"] = result.GroupsId;
                return Redirect("/Home/Index"); //Asp3
            }
        }

        public ActionResult GetRefLinks(string lastname)
        {
            string message = null;
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync($"{"https://localhost:44362/api/Admin/refLinks?lastname=" + lastname }").Result;
                message = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<RefLinksViewModel>(message);
                if (result == null)
                {
                    return Redirect("/Home/Index");
                }
                TempData["RefGroupsId"] = result.groupsId;
                TempData["RefLastname"] = result.refLastname;
                return Redirect("/Home/Index"); //Asp3
            }
        }

        public ActionResult GetQR(string link)
        {
            //var result = new RegLinksViewModel();
            //result.QRCode = "https://localhost:44362/api/Admin/regQR";
            //TempData["QrCodeUri"] = result.QRCode;
            return Redirect("/api/Admin/regQR?link="+link);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
