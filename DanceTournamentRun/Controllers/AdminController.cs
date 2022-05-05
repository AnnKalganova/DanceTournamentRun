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
                ViewBag.groups = GetGroupViewModels(groups);
                return PartialView("Groups");
            }
          
            //TODO: исправить на вывод ошибки
            return PartialView("Error");
        }

        private ICollection<GroupViewModal> GetGroupViewModels(IQueryable<Group> groups)
        {
            ICollection<GroupViewModal> groupViews = new List<GroupViewModal>();
            foreach (var group in groups)
            {
                GroupViewModal groupView = new GroupViewModal() ;
                groupView.GroupId = group.Id;
                groupView.Name = group.Name;
                groupView.Number = group.Number;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    groupView.Dances = db.GetDances(group.Id);
                }
                groupViews.Add(groupView);
                //TODO: отсортировать по Number
            }
            return groupViews;
        }

        [HttpPost]
        public async Task<ActionResult> AddGroup(CreateGroupModel model)
        {
            if (ModelState.IsValid)
            {
                var numbers = _context.Groups.Where(g => g.TournamentId == model.TournamentId).Select(x => x.Number);
                var newNumber = numbers.Count() != 0 ? numbers.Max() + 1 : 1;
                Group group = new Group { Name = model.Name, Number = newNumber, TournamentId = model.TournamentId };
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                if( model.Dances.Count() != 0)
                {
                    foreach (var dance in model.Dances)
                    {
                        Dance newDance = new Dance { Name = dance };
                        _context.Dances.Add(newDance);
                        await _context.SaveChangesAsync();

                        GroupsDance groupsDance = new GroupsDance { GroupId = group.Id, DanceId = newDance.Id };
                        _context.GroupsDances.Add(groupsDance);
                        await _context.SaveChangesAsync();
                    }
                }
                
                return RedirectToAction("ViewGroups", new { tournId = model.TournamentId });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGroup(long? groupId)
        {
            if (groupId != null)
            {
                //delete dances
                var dancesId = _context.GroupsDances.Where(x => x.GroupId == groupId).Select(p => p.DanceId).ToList();

                if (dancesId.Count() != 0)
                {
                    foreach (var danceId in dancesId)
                    {
                        var dance = _context.Dances.Find(danceId);
                        _context.Dances.Remove(dance);
                        await _context.SaveChangesAsync();
                    }
                }

                ////delete group
                var group = _context.Groups.Find(groupId);
                var tournId = group.TournamentId;
                var number = group.Number;
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                //change numbers
                var updateGr = _context.Groups.Where(p => p.TournamentId == tournId && p.Number > number).ToList();

                foreach (var gr in updateGr)
                {
                    gr.Number = gr.Number - 1;
                    await _context.SaveChangesAsync();
                }

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
