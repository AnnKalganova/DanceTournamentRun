using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Pair
    {
        public Pair()
        {
            Scores = new HashSet<Score>();
            Results = new HashSet<Result>();
        }

        [Required(ErrorMessage = "Не id")]
        public long Id { get; set; }
        public long GroupId { get; set; }

        [Required(ErrorMessage = "Не указано имя партнера")]
        public string Partner1FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия партнера")]
        public string Partner1LastName { get; set; }

        [Required(ErrorMessage = "Не указано имя партнерши")]
        public string Partner2FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия партнерши")]
        public string Partner2LastName { get; set; }
        public int? Number { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<Score> Scores { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
