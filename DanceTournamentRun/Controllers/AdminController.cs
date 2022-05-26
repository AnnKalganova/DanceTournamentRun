using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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
using Microsoft.EntityFrameworkCore;

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
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                return RedirectToAction("CreateTournament");
            }
            if ((bool)tournament.IsTournamentRun)
            {
                return RedirectToAction("Index", "RunTournament");
            }
            ViewBag.Login = _context.Users.Where(u => u.Id == tournament.UserId).Select(p => p.Login).FirstOrDefault();
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
                GroupViewModal groupView = new GroupViewModal() { GroupId = group.Id, Name = group.Name.Replace('_', ' '), Number = group.Number } ;
               
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
                Group group = new Group { Name = model.Name.Replace(' ', '_'), Number = newNumber, TournamentId = model.TournamentId };
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
                string editGrName = editGroup.Name.Replace(' ', '_');
                var oldgr = _context.Groups.Find(editGroup.GroupId);
                oldgr.Name = editGrName != oldgr.Name ? editGrName : oldgr.Name;
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
                            pairsInGr.Add(lastGrId, new List<Pair>(parsForDict));
                            lastGrId = currGrId;
                            parsForDict.Clear();
                            parsForDict.Add(pair);
                        }
                    }
                    pairsInGr.Add(lastGrId, parsForDict);
                }
                List<Group> spaced = new List<Group>();
                foreach (var gr in pairGroups)
                {
                    Group newGr = new Group() { Id = gr.Id, Name  = gr.Name, Number = gr.Number, CompetitionState = gr.CompetitionState, IsRegistrationOn  = gr.IsRegistrationOn, TournamentId=gr.TournamentId };
                    newGr.Name = newGr.Name.Replace('_', ' ');
                    spaced.Add(newGr);
                }
                ViewBag.tournamentId = tournId;
                ViewBag.pairGroups = spaced;
                ViewBag.unspacedGroups = pairGroups;
                ViewBag.pairs = pairsInGr;
                return PartialView("Pairs");
            }
            //TODO: исправить на вывод ошибки
            return PartialView("Error");
        }

        [HttpPost]
        public async Task<ActionResult> AddPair(CreatePairModel model)
        {
            if (ModelState.IsValid)
            {
                int countSimilarP1, countSimilarP2;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    countSimilarP1 = db.FindSimilarPartner(model.GroupId, model.Partner1LastName, model.Partner1FirstName);
                    countSimilarP2 = db.FindSimilarPartner(model.GroupId, model.Partner2LastName, model.Partner2FirstName);
                }
                if(countSimilarP1 == 0 && countSimilarP2 == 0)
                {
                    Pair newPair = new Pair()
                    {
                        GroupId = model.GroupId,
                        Partner1LastName = model.Partner1LastName,
                        Partner1FirstName = model.Partner1FirstName,
                        Partner2LastName = model.Partner2LastName,
                        Partner2FirstName = model.Partner2FirstName
                    };
                    _context.Pairs.Add(newPair);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewPairs", new { tournId = model.TournamentId });
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> EditPair(Pair editPair)
        {
            if (ModelState.IsValid)
            {
                var oldPair = _context.Pairs.Find(editPair.Id);
                if(oldPair.Partner1LastName!= editPair.Partner1LastName || oldPair.Partner1FirstName != editPair.Partner1FirstName)
                {
                    int countSimilar;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        countSimilar = db.FindSimilarPartner(oldPair.GroupId, editPair.Partner1LastName, editPair.Partner1FirstName);
                    }
                    if (countSimilar == 0)
                    {
                        oldPair.Partner1LastName = editPair.Partner1LastName;
                        oldPair.Partner1FirstName = editPair.Partner1FirstName;
                        await _context.SaveChangesAsync();
                    }
                    else return NotFound();
                }
                else if (oldPair.Partner2LastName != editPair.Partner2LastName || oldPair.Partner2FirstName != editPair.Partner2FirstName)
                {
                    int countSimilar;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        countSimilar = db.FindSimilarPartner(oldPair.GroupId, editPair.Partner2LastName, editPair.Partner2FirstName);
                    }
                    if (countSimilar == 0)
                    {
                        oldPair.Partner2LastName = editPair.Partner2LastName;
                        oldPair.Partner2FirstName = editPair.Partner2FirstName;
                        await _context.SaveChangesAsync();
                    }
                    else return NotFound();
                }
                var group = _context.Groups.Find(oldPair.GroupId);
                return RedirectToAction("ViewPairs", new { tournId = group.TournamentId });
            }
            return NotFound();
        }

        [HttpPost] 
        public async Task<ActionResult> DaletePair(long? pairId)
        {
            if (pairId != null)
            {
                var pair = _context.Pairs.FirstOrDefault(p => p.Id == pairId);
                if (pair != null)
                {
                    var gr = _context.Groups.Find(pair.GroupId);
                    _context.Pairs.Remove(pair);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewPairs", new { tournId = gr.TournamentId });
                }
            }
            return NotFound();
        }

        public PartialViewResult ViewReferees(long? tournId)
        {
            if (tournId != null)
            {
                var groups = _context.Groups.Where(p => p.TournamentId == tournId).ToList();
                List<User> referees = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    referees = db.GetRefereesByTourn((long)tournId);
                }
                List<RefereeViewModel> refereeViews = new List<RefereeViewModel>();
                foreach (var referee in referees)
                {
                    var refGroupsId = _context.UsersGroups.Where(r => r.UserId == referee.Id).Select(p=>p.GroupId).ToArray();
                    RefereeViewModel viewModel = new RefereeViewModel() { Id = referee.Id, LastName = referee.LastName, FirstName = referee.FirstName, GroupsId = refGroupsId };
                    refereeViews.Add(viewModel);
                }
                List<Group> spaced = new List<Group>();
                foreach (var gr in groups)
                {
                    Group newGr = new Group() { Id = gr.Id, Name = gr.Name, Number = gr.Number, CompetitionState = gr.CompetitionState, IsRegistrationOn = gr.IsRegistrationOn, TournamentId = gr.TournamentId };
                    newGr.Name = newGr.Name.Replace('_', ' ');
                    spaced.Add(newGr);
                }
                ViewBag.tournamentId = tournId;
                ViewBag.unspacedGroups = groups;
                ViewBag.refGroups = spaced;
                ViewBag.referees = refereeViews;
                return PartialView("Referees");
            }
            return PartialView("Error");
        }

        [HttpPost]
        public async Task<ActionResult> AddReferee(CreateRefereeModel model)
        {
            if (ModelState.IsValid)
            {
                var roleId = _context.Roles.Where(r => r.Name == "referee").Select(d => d.Id).FirstOrDefault();
                User referee = new User() {
                    LastName = model.LastName, 
                    FirstName = model.FirstName,
                    Login = model.Login, 
                    Password = Guid.NewGuid().ToString(),
                    RoleId = roleId,
                    SecurityToken = Guid.NewGuid().ToString()
                };
                _context.Users.Add(referee);
                await _context.SaveChangesAsync();

                UsersTournament usersTournament = new UsersTournament() { TournamentId = model.TournamentId, UserId = referee.Id };
                _context.UsersTournaments.Add(usersTournament);
                await _context.SaveChangesAsync();

                foreach(var grId in model.GroupsId)
                {
                    UsersGroup usersGroup = new UsersGroup() { UserId = referee.Id, GroupId = grId };
                    _context.UsersGroups.Add(usersGroup);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ViewReferees", new { tournId = model.TournamentId });

            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> EditReferee(EditRefereeModel model)
        {
            if (ModelState.IsValid)
            {
                var referee = _context.Users.Include(p => p.Role).FirstOrDefault(u=>u.Id == model.Id);
                if (referee!=null && referee.Role.Name == "referee")
                {
                    referee.LastName = model.LastName;
                    referee.FirstName = model.FirstName;
                    await _context.SaveChangesAsync();

                    var oldGrId = _context.UsersGroups.Where(r => r.UserId == referee.Id).Select(p => p.GroupId).ToArray();
                    var grsToAddId = model.GroupsId.Except(oldGrId);
                    var grsToDelId = oldGrId.Except(model.GroupsId);

                    foreach( var grAddId in grsToAddId)
                    {
                        UsersGroup usersGroup = new UsersGroup() { UserId = referee.Id, GroupId = grAddId };
                        _context.UsersGroups.Add(usersGroup);
                        await _context.SaveChangesAsync();
                    }
                    foreach (var grDelId in grsToDelId)
                    {
                        UsersGroup usersGroup = _context.UsersGroups.First(u => u.GroupId == grDelId && u.UserId == referee.Id);
                        _context.UsersGroups.Remove(usersGroup);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("ViewReferees", new { tournId = model.TournamentId });
                }

            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> DaleteReferee(long? refereeId, long? tournId)
        {
            if(refereeId!=null && tournId != null)
            {
                User referee = _context.Users.Find(refereeId);
                UsersTournament tournament = _context.UsersTournaments.First(t => t.UserId == referee.Id);
                var groups = _context.UsersGroups.Where(g => g.UserId == referee.Id);
                _context.UsersTournaments.Remove(tournament);
                foreach(var group in groups)
                {
                    _context.UsersGroups.Remove(group);
                }
                _context.SaveChanges();
                _context.Users.Remove(referee);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewReferees", new { tournId = tournId });
            }
            return NotFound();
        }
        public PartialViewResult ViewRegistrators(long? tournId)
        {
            if (tournId != null)
            {
                List<User> registrators = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    registrators = db.GetRegistratorsByTournId((long)tournId);
                }
                ViewBag.tournamentId = tournId;
                ViewBag.registrators = registrators;
                return PartialView("Registrators");
            }
            return PartialView("Error");
        }

        [HttpPost]
        public async Task<ActionResult> AddRegistrator(CreateRegistratorModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO проверка что имейл уникальный
                var roleId = _context.Roles.Where(r => r.Name == "registrator").Select(d => d.Id).FirstOrDefault();
                User registrator = new User()
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Login = model.Login,
                    Password = Guid.NewGuid().ToString(),
                    RoleId = roleId,
                    SecurityToken = Guid.NewGuid().ToString()
                };
                _context.Users.Add(registrator);
                await _context.SaveChangesAsync();

                UsersTournament usersTournament = new UsersTournament() { TournamentId = model.TournamentId, UserId = registrator.Id };
                _context.UsersTournaments.Add(usersTournament);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewRegistrators", new { tournId = model.TournamentId });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> EditRegistrator(CreateRegistratorModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Id != model.Id);
                if (user == null)
                {
                    User oldUser = _context.Users.Find(model.Id);
                    oldUser.Login = model.Login;
                    oldUser.LastName = model.LastName;
                    oldUser.FirstName = model.FirstName;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewRegistrators", new { tournId = model.TournamentId });
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> DaleteRegistrator(long? regId, long? tournId)
        {
            if (regId != null && tournId != null)
            {
                User registrator = _context.Users.Find(regId);
                UsersTournament tournament = _context.UsersTournaments.First(t => t.UserId == registrator.Id);
                _context.UsersTournaments.Remove(tournament);
                _context.SaveChanges();
                _context.Users.Remove(registrator);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewRegistrators", new { tournId = tournId });
            }
            return NotFound();
        }
        public async Task<ActionResult> RunTourn(long? tournId)
        {
            if(tournId.HasValue)
            {
                var tourn = _context.Tournaments.Find(tournId);
                tourn.IsTournamentRun = true;
                await _context.SaveChangesAsync();

                List<User> registrators = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    registrators = db.GetRegistratorsByTournId((long)tournId);
                }

                var groups = _context.Groups.Where(p => p.TournamentId == tournId).OrderBy(k=>k.Number).ToList();
                var regCount = registrators.Count;
                for (var i = 0; i < groups.Count; i++)
                {
                    var regID = registrators[i % regCount].Id;
                    var grId = groups[i].Id;
                    UsersGroup usGr = new UsersGroup() { UserId = regID, GroupId = grId };
                    _context.UsersGroups.Add(usGr);
                    await _context.SaveChangesAsync();
                }
                groups[0].CompetitionState = 1;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "RunTournament");
            }
            return NotFound();
        }

        //public ActionResult GetRegLinks()
        //{
        //    string message = null;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var response = client.GetAsync($"{"https://localhost:44362/api/Admin/regLinks" }").Result;
        //        message = response.Content.ReadAsStringAsync().Result;
        //        var result  = JsonConvert.DeserializeObject<RegLinksViewModel>(message);
        //        Console.WriteLine(result);
        //        TempData["RegGroupsId"] = result.GroupsId;
        //        return Redirect("/Home/Index"); //Asp3
        //    }
        //}

        //public ActionResult GetRefLinks(string lastname)
        //{
        //    string message = null;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var response = client.GetAsync($"{"https://localhost:44362/api/Admin/refLinks?lastname=" + lastname }").Result;
        //        message = response.Content.ReadAsStringAsync().Result;
        //        var result = JsonConvert.DeserializeObject<RefLinksViewModel>(message);
        //        if (result == null)
        //        {
        //            return Redirect("/Home/Index");
        //        }
        //        TempData["RefGroupsId"] = result.groupsId;
        //        TempData["RefLastname"] = result.refLastname;
        //        return Redirect("/Home/Index"); //Asp3
        //    }
        //}

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
