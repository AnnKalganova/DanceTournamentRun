using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class EditRefereeModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string FirstName { get; set; }
        public long TournamentId { get; set; }
        public long[] GroupsId { get; set; }
    }
}
