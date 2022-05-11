using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class CreatePairModel
    {
        [Required(ErrorMessage = "Не выбрана группа")]
        public long GroupId { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Partner1FirstName { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Partner1LastName { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Partner2FirstName { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Partner2LastName { get; set; }

        public long TournamentId { get; set; }
    }
}
