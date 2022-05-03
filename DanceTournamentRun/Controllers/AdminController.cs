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
            // var userId = UserHelper.GetUserId();
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                //redirect to create tournament
                return RedirectToAction("CreateTournament");
            }
            if ((bool)tournament.IsTournamentRun)
            {
                //redirect to tournament run 

            }

            return View("SetupTournament");


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
                var userId = UserHelper.GetUserId();
                Tournament tournament = new Tournament { Name = model.Name, UserId = userId, IsTournamentRun = false };
                _context.Tournaments.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
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
