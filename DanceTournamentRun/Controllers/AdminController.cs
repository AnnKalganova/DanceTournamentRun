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
             var sortedGr = groups.OrderBy(gr => gr.Number);
            foreach (var group in sortedGr)
            {
                GroupViewModal groupView = new GroupViewModal() { GroupId = group.Id, Name = group.Name, Number = group.Number } ;
               
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
        public async Task<ActionResult> EditGroup(EditGroupModel editGroup)
        {
            if (ModelState.IsValid)
            {
                var oldgr = _context.Groups.Find(editGroup.GroupId);
                oldgr.Name = editGroup.Name != oldgr.Name ? editGroup.Name : oldgr.Name;
                oldgr.Number = editGroup.Number != oldgr.Number ? GetNewNumber(oldgr, editGroup.Number) : oldgr.Number;
                await _context.SaveChangesAsync();

                var dancesId = _context.GroupsDances.Where(x => x.GroupId == oldgr.Id).Select(p => p.DanceId).ToList();
                EditDances(editGroup.Dances, dancesId, oldgr.Id);

                return RedirectToAction("ViewGroups", new { tournId = oldgr.TournamentId });
            }
            return NoContent();
        }

        private int GetNewNumber(Group oldGr, int editNumber)
        {
            var newNumber = 0;
            var param = 0;
            if(editNumber > oldGr.Number)
            {
                var numbers = _context.Groups.Where(g => g.TournamentId == oldGr.TournamentId).Select(x => x.Number);
                var maxNum = numbers.Max();
                newNumber = editNumber > maxNum ? maxNum : editNumber;
                param = 1;
                EditNumbers(oldGr.Number + 1, newNumber, oldGr.TournamentId, param);
            }
            else
            {
                newNumber = editNumber;
                param = 2;
                EditNumbers(newNumber, oldGr.Number - 1, oldGr.TournamentId,param);
            }
            return newNumber;
        }

        private async void EditNumbers(int fromNum, int toNum, long tournId, int param)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var groups = db.Groups.Where(p => p.TournamentId == tournId && (p.Number >= fromNum && p.Number <= toNum)).ToList();
                switch (param)
                {
                    case 1:
                        foreach (var gr in groups)
                        {
                            gr.Number = gr.Number - 1;
                            await db.SaveChangesAsync();
                        }
                        break;
                    case 2:
                        foreach (var gr in groups)
                        {
                            gr.Number = gr.Number + 1;
                            await db.SaveChangesAsync();
                        }
                        break;
                }
            }
        }

        private void EditDances(string[] newDances,List<long> dancesId, long grId)
        {
            if(newDances.Length > dancesId.Count())
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var i = 0;
                    foreach (var id in dancesId)
                    {
                        var dance = db.Dances.Find(id);
                        dance.Name = newDances[i] != dance.Name ? newDances[i] : dance.Name;
                        db.SaveChangesAsync();
                        i++;
                    }
                    for (var k = i; k < newDances.Length; k++)
                    {
                        Dance addDance = new Dance { Name = newDances[k] };
                        db.Dances.Add(addDance);
                        db.SaveChangesAsync();

                        GroupsDance groupsDance = new GroupsDance { GroupId = grId, DanceId = addDance.Id };
                        db.GroupsDances.Add(groupsDance);
                        db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var i = 0;
                    foreach (var dName in newDances)
                    {
                        var dance = db.Dances.Find(dancesId[i]);
                        dance.Name = dName != dance.Name ? dName : dance.Name;
                        db.SaveChangesAsync();
                        i++;
                    }
                    for (var k = i; k < dancesId.Count(); k++)
                    {
                        var delDance = db.Dances.Find(dancesId[k]);
                        db.Dances.Remove(delDance);
                        db.SaveChangesAsync();
                    }
                }
                   
            }
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

        public PartialViewResult ViewPairs(long? tournId)
        {
            if (tournId != null)
            {
                var pairGroups = _context.Groups.Where(p => p.TournamentId == tournId).ToList();
                List<Pair> pairs;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    pairs = db.GetPairsByTourn((long)tournId);
                }

                Dictionary<long, List<Pair>> pairsInGr = new Dictionary<long, List<Pair>>();

                if(pairs.Count() != 0)
                {
                    var lastGrId = pairs.First().GroupId;
                    List<Pair> parsForDict = new List<Pair>();
                    foreach (var pair in pairs)
                    {
                        var currGrId = pair.GroupId;
                        if (currGrId == lastGrId)
                        {
                            parsForDict.Add(pair);
                        }
                        else
                        {
                            pairsInGr.Add(lastGrId, parsForDict);
                            lastGrId = currGrId;
                            parsForDict.Clear();
                            parsForDict.Add(pair);
                        }
                    }
                    pairsInGr.Add(lastGrId, parsForDict);
                }

                ViewBag.tournamentId = tournId;
                ViewBag.pairGroups = pairGroups;
                ViewBag.pairs = pairsInGr;
                return PartialView("Pairs");
            }
            //TODO: исправить на вывод ошибки
            return PartialView("Error");
        }

        private ICollection<GroupViewModal> GetPairViewModels(IQueryable<Group> groups)
        {
            ICollection<GroupViewModal> groupViews = new List<GroupViewModal>();
            var sortedGr = groups.OrderBy(gr => gr.Number);
            foreach (var group in sortedGr)
            {
                GroupViewModal groupView = new GroupViewModal() { GroupId = group.Id, Name = group.Name, Number = group.Number };

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    groupView.Dances = db.GetDances(group.Id);
                }
                groupViews.Add(groupView);
                //TODO: отсортировать по Number
            }
            return groupViews;
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
