using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun
{
    public class PairViewModel
    {
        /// <summary>
        /// View Model Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Club Id
        /// </summary>
        public long ClubId { get; set; }

        /// <summary>
        /// Club Name
        /// </summary>
        public string ClubName { get; set; }

        /// <summary>
        /// Club City
        /// </summary>
        public string ClubCity { get; set; }

        /// <summary>
        /// Club ViewModel
        /// </summary>
        //public ClubViewModel Club { get; set; }

        /// <summary>
        /// Coach Id
        /// </summary>
        public long CoachId { get; set; }

        /// <summary>
        /// Coach Name
        /// </summary>
        public string CoachName { get; set; }

        /// <summary>
        /// Coach Name
        /// </summary>
        public string CoachSurname { get; set; }

        /// <summary>
        /// Group Id
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Invitation Id
        /// </summary>
        
        public long PartnerMaleId { get; set; }

        /// <summary>
        /// Partner Male Name
        /// </summary>
        public string PartnerMaleName { get; set; }

        /// <summary>
        /// Partner Male Surname
        /// </summary>
        public string PartnerMaleSurname { get; set; }

        /// <summary>
        /// Partner Female Id
        /// </summary>
        public long PartnerFemaleId { get; set; }

        /// <summary>
        /// Partner Female Name
        /// </summary>
        public string PartnerFemaleName { get; set; }

        /// <summary>
        /// Partner Female Surname
        /// </summary>
        public string PartnerFemaleSurname { get; set; }

        public int Number { get; set; }
    }
}
