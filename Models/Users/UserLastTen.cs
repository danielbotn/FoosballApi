using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FoosballApi.Enums;

namespace FoosballApi.Models
{
    public class UserLastTen
    {
        public ETypeOfMatch TypeOfMatch { get; set; }
        public string TypeOfMatchName { get; set; }
        public int UserId { get; set; }
        public int? TeamMateId { get; set; }

        public int MatchId { get; set; }

        public int OpponentId { get; set; }

        public int? OpponentTwoId { get; set; }

        public string OpponentOneFirstName { get; set; }

        public string OpponentOneLastName { get; set; }

        public string OpponentTwoFirstName { get; set; }
        
        public string OpponentTwoLastName { get; set; }

        public int UserScore { get; set; }

        public int OpponentUserOrTeamScore { get; set; }

        public DateTime DateOfGame { get; set; }

    }
}