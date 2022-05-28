using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.Controllers
{
    [Authorize(Roles = "admin")]
    public class RunTournamentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public RunTournamentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                return RedirectToAction("CreateTournament", "Admin");
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
                    return PartialView("FirstStep", tourn);
                }
                else if (!(bool)tourn.IsSecondStepOver)
                {
                    return RedirectToAction("GetGroupProcessData", "RunTournament", new { tournId = tourn.Id } );
                }
                else 
                {
                    return PartialView("EndStep", tourn);
                }
            }
            return PartialView("Error");
        }

        public async Task<ActionResult> GetRegQR(long? tournId)
        {
            if (tournId != null)
            {
                //сформировать qr +вывести
                List<User> registrators = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    registrators = db.GetRegistratorsByTournId((long)tournId);
                }

                List<RegViewModel> regViews = new List<RegViewModel>();
                foreach (var reg in registrators)
                {
                    var link = _configuration["IpForQR"];
                    link += "Registration/" + reg.SecurityToken;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    RegViewModel regView = new RegViewModel() { LastName = reg.LastName, Firstname = reg.FirstName, QR = BitmapToBytesCode(qrCodeImage) };
                    regViews.Add(regView);
                }
                ViewBag.regViews = regViews;
                return View("RegQRs");
                // строка прогресса регистрации ViewComponent??
            }
            return View("Error");
        }

        public async Task<ActionResult> GetRefereeQR(long? tournId)
        {
            if (tournId != null)
            {
                List<User> referees = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    referees = db.GetRefereesByTourn((long)tournId);
                }

                List<RegViewModel> refViews = new List<RegViewModel>();
                foreach (var referee in referees)
                {
                    var link = _configuration["IpForQR"];
                    link += "Referee/" + referee.SecurityToken;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    RegViewModel refView = new RegViewModel() { LastName = referee.LastName, Firstname = referee.FirstName, QR = BitmapToBytesCode(qrCodeImage) };
                    refViews.Add(refView);
                }
                ViewBag.regViews = refViews;
                return View("RefereeQRs");
            }
            return View("Error");
        }

        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public IActionResult UpdateRegProgress(long Id)
        {
            return ViewComponent("RegistrationProgress",
                new
                {
                    tournId = Id
                });
        }

        public async Task<ActionResult> GoToStepTwo(long? tournId)
        {
            if (tournId.HasValue)
            {
                var tourn = _context.Tournaments.Find(tournId);
                tourn.IsFirstStepOver = true;
                await _context.SaveChangesAsync();

                var firstGr = _context.Groups.Where(gr => gr.TournamentId == tourn.Id).OrderBy(k => k.Number).First();
                firstGr.CompetitionState = 1;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "RunTournament");
            }
            return NotFound();
        }

        public PartialViewResult GetGroupProcessData(long? tournId)
        {
            if(tournId != null)
            {
                Group group = new Group();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    group = db.GetCurrentGroup((long)tournId);
                }
                group.Name = group.Name.Replace('_', ' ');
                switch (group.CompetitionState)
                {
                    case 1:
                        return PartialView("HeatsFormation", group);
                    case 2:
                        var nextGr = _context.Groups.FirstOrDefault(g => g.TournamentId == group.TournamentId && g.Number == group.Number + 1);
                        ViewBag.nextGr = nextGr == null ? null : nextGr.Name.Replace("_"," ");
                        return PartialView("RefereeProcess", group);
                }
            }
            return PartialView("Error");
        }

        public IActionResult GetHeats(long groupId)
        {
            var pair = _context.Pairs.FirstOrDefault(p => p.GroupId == groupId);
            if (pair == null)
            {
                return NotFound();
            }
            else if (isPairScoreExist(pair))
            {
                return ViewComponent("GroupHeats", new
                {
                    groupId = groupId
                });
            }
            var pairs = _context.Pairs.Where(p => p.GroupId == groupId).ToList();
            int[] heats = SetHeats(pairs.Count);
            ICollection<Dance> dances;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                dances = db.GetDances((long)groupId);
            }
            List<User> groupReferees = new List<User>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groupReferees = db.GetRefereesByGroupId(groupId);
            }
            var refPrIds = CreateRefProgress(groupReferees, dances, heats.Length);
            CreateScoreRecords(refPrIds, pairs, heats);
            return ViewComponent("GroupHeats", new
            {
                groupId = groupId
            });

        }

        private bool isPairScoreExist(Pair pair)
        {
            var score = _context.Scores.FirstOrDefault(s => s.PairId == pair.Id);
            return score != null ? true : false; 
            
        }
        private int[] SetHeats(int pairsCount)
        {
            var heatsCount = pairsCount / 8;
            if(heatsCount == 0)
            {
                return new int[] { pairsCount % 8 };
            }
            int[] heats;
            heats = new int[heatsCount];
            Array.Fill(heats, 8);
            var lastHeat = pairsCount % 8;
            if (lastHeat != 0)
            {
                if (lastHeat < 5)
                {
                    var i = 0;
                    while (lastHeat < 5)
                    {
                        heats[i] -= 1;
                        lastHeat++;
                        i++;
                        if (i == heats.Length)
                        {
                            i = 0;
                        }
                    }
                }
                heats.Append(lastHeat);
            }
            return heats;
        }

        private Dictionary<int, List<long>> CreateRefProgress(List<User> referees, ICollection<Dance> dances, int heatsCount)
        {
            Dictionary<int, List<long>> refProgIds = new Dictionary<int,List<long>>();
            foreach (var dance in dances) {
                for (var i = 1; i < heatsCount + 1; i++)
                {
                    if (!refProgIds.ContainsKey(i))
                        refProgIds.Add(i, new List<long>());
                    foreach(var referee in referees)
                    {
                        RefereeProgress refereeProgress = new RefereeProgress() { UserId = referee.Id, DanceId = dance.Id, Heat = i };
                        _context.RefereeProgresses.Add(refereeProgress);
                        _context.SaveChanges();
                        refProgIds[i].Add(refereeProgress.Id);
                    }
                }
            }
            return refProgIds;
        }

        private void CreateScoreRecords(Dictionary<int,List<long>> refProgIds, List<Pair> pairs, int[] pairsInHeatCount)
        {
            pairs.OrderBy(p => p.Number);
            var minPairIndex = 0;
            for(var i = 0; i < pairsInHeatCount.Length ; i++)
            {
                var PairCount = pairsInHeatCount[i];
                foreach(var refId in refProgIds[i+1])
                {
                    for (var k = minPairIndex; k < minPairIndex + PairCount; k++)
                    {
                        Score score = new Score() { PairId = pairs[k].Id, ProgressId = refId, Score1 = 0 };
                        _context.Scores.Add(score);
                        _context.SaveChanges();
                    }
                }
                minPairIndex += PairCount;
            }
        } 

        public ActionResult GetHeatsToPrint(long? groupId)
        {
            if(groupId == null)
            {
                return NotFound();
            }
            var danceId = _context.GroupsDances.First(d => d.GroupId == groupId).Id;
            var refereeId = _context.GetRefereesByGroupId((long)groupId)[0].Id;
            var refProgresses = _context.GetHeats(refereeId, danceId);
            Dictionary<int, List<int>> heats = new Dictionary<int, List<int>>();
            foreach (var progress in refProgresses)
            {
                heats.Add(progress.Heat, new List<int>());
                var pairsInHeat = _context.GetPairsByRefProgress(progress.Id);
                foreach (var pair in pairsInHeat)
                {
                    heats[progress.Heat].Add((int)pair.Number);
                }
            }
            return View("HeatsToPrint", heats);
        }

        public async Task<ActionResult> GoToRefereeProcessStep(long groupId)
        {
            var group = _context.Groups.Find(groupId);
            if(group == null || group.CompetitionState != 1)
            {
                return NotFound();
            }
            group.CompetitionState = 2;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "RunTournament");
        }

        public IActionResult UpdateRefProgress(long Id)
        {
            return ViewComponent("RefereeProcess",
                new
                {
                    groupId = Id
                });
        }

        public async Task<ActionResult> GetResults(long groupId)
        {
            //TODO разделить на более мелкие 
            var pairs = _context.Pairs.Where(p => p.GroupId == groupId).ToList();
            if(pairs == null)
            {
                return NotFound(); 
            }
            Dictionary<long, int> pairsScores = new Dictionary<long, int>();
            foreach(var pair in pairs)
            {
                var score = await _context.GetPairScore(pair.Id);
                pairsScores.Add(pair.Id, score);
            }
            pairsScores = pairsScores.OrderByDescending(p => p.Value).ToDictionary(x => x.Key, x => x.Value);
            var isResultExist = _context.Results.FirstOrDefault(r => r.PairId == pairsScores.ElementAt(0).Key);
            if(isResultExist != null)
            {
                return ViewComponent("Results",
                new
                {
                    orderedPairs = pairsScores
                });
            }
            List<int> resDevisions = new List<int>();
            int defaultDevision = pairs.Count / 3;
            int index = defaultDevision;
            //TODO проверка чтоб разделение не дощло до конца 
            while(index < pairs.Count)
            {
                if (pairsScores.ElementAt(index-1).Value == pairsScores.ElementAt(index).Value)
                {
                    index -= 1;
                }
                else
                {
                    resDevisions.Add(index);
                    defaultDevision *= 2;
                    index = defaultDevision;
                }
            }
            resDevisions.Add(pairs.Count);

            var position = 1;
            var i = 0;
            foreach(var dev in resDevisions)
            {
                while (i < dev)
                {
                    Result result = new Result() { PairId = pairsScores.ElementAt(i).Key, Position = position };
                    _context.Results.Add(result);
                    await _context.SaveChangesAsync();
                    i++;
                }
                position++;
            }
            return ViewComponent("Results",
                new
                {
                    orderedPairs = pairsScores
                });
        }

        public ActionResult GetResultsToPrint(string results)
        {
            List<ResultsViewModel> resultsViews = JsonConvert.DeserializeObject<List<ResultsViewModel>>(results);
            return View("ResultsToPrint", resultsViews);
        }

        public async Task<ActionResult> GoToNextGroup(long groupId, long nextGrId)
        {
            var currGr = _context.Groups.Find(groupId);
            var nextGr = _context.Groups.Find(nextGrId);
            if (currGr == null || nextGr == null)
                return NotFound();
            currGr.CompetitionState = 3;
            nextGr.CompetitionState = 1;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "RunTournament");
        }

        public async Task<ActionResult> GoToEndStep(long tournId)
        {
            var tournament = _context.Tournaments.Find(tournId);
            if (tournament == null)
                return NotFound();
            tournament.IsSecondStepOver = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "RunTournament");
            
        }

        public async Task<ActionResult> DeleteTournament(long tournId)
        {
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament.Id != tournId)
            {
                return NotFound();
            }
            var groups = _context.Groups.Where(gr=> gr.TournamentId == tournId).ToList();
            foreach (var group in groups)
            {
                var dances = _context.GetDances(group.Id);
                foreach (var dance in dances)
                {
                    _context.Dances.Remove(dance);
                    await _context.SaveChangesAsync();
                }
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            var referees = _context.GetAllTournUsers(tournId);
            foreach (var referee in referees)
            {
                _context.Users.Remove(referee);
                await _context.SaveChangesAsync();
            }
            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "RunTournament");
        }
    }
}


       
