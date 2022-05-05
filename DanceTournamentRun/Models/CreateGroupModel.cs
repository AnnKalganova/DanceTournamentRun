using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class CreateGroupModel
    {
       
        [Required(ErrorMessage = "Не указано название")]
        public string Name { get; set; }

        public long TournamentId { get; set; }

        public string[] Dances { get; set; } 

    }
}
