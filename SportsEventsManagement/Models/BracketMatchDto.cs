namespace SportsEventsManagement.Models
{
    public class BracketMatchDto
    {
        public int Id { get; set; }
        public DateTime DateMatch { get; set; }
        public int Tour { get; set; }

        // Flattened Data: Just Strings and Ints
        public string HomeTeamName { get; set; } = "TBD";
        public string AwayTeamName { get; set; } = "TBD";
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}