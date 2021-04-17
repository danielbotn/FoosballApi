using System;

namespace FoosballApi.Dtos.Matches
{
    public class FreehandMatchesReadDto
    {
        public int Id { get; set; }

        public int PlayerOneId { get; set; }

        public int PlayerTwoId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int PlayerOneScore { get; set; }

        public int PlayerTwoScore { get; set; }

        public int UpTo { get; set; }

        public bool GameFinished { get; set; }

        public bool GamePaused { get; set; }
    }
}