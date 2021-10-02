namespace FoosballApi.Models.Users
{
    public class UserStats
    {
        public int UserId { get; set; }
        public int TotalMatches { get; set; }
        public int TotalMatchesWon { get; set; }
        public int TotalMatchesLost { get; set; }
        public int TotalGoalsScored { get; set; }
        public int TotalGoalsReceived { get; set; }
    }
}