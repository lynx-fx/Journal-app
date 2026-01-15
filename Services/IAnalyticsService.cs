using JournalApp.Models;

namespace JournalApp.Services;

public class StreakData
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalEntries { get; set; }
    public List<DateTime> MissedDays { get; set; } = new();
}

public class MoodStats
{
    public Moods Category { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class MoodDetailStat
{
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public int Count { get; set; }
}

public interface IAnalyticsService
{
    Task<StreakData> GetStreakDataAsync();
    Task<List<MoodStats>> GetMoodDistributionAsync();
    Task<List<MoodDetailStat>> GetTopMoodDetailsAsync();
    Task<List<KeyValuePair<string, int>>> GetTopTagsAsync();
    Task<List<KeyValuePair<DateTime, int>>> GetWordCountTrendAsync(int days = 30);
}
