using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DanceTournamentRun.Models
{
    public class GroupViewModal
    {
        public long GroupId { get; set; }

        [Required(ErrorMessage = "Не указан порядок")]
        public int Number { get; set; }
        [Required(ErrorMessage = "Не указано название")]
        [JsonProperty(PropertyName = "Data1")]
        public string Name { get; set; }
        public ICollection<Dance> Dances { get; set; }

    }
}
