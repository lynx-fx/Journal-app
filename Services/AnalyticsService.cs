using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDatabase _db;

    public AnalyticsService(AppDatabase db)
    {
        _db = db;
    }

    private async Task Init()
    {
        await _db.Init();
    }

    public async Task<StreakData> GetStreakDataAsync()
    {
        await Init();
        var allDates = (await _db.Database.Table<Journals>().ToListAsync())
                       .Select(j => j.Date.Date)
                       .Distinct()
                       .OrderByDescending(d => d)
                       .ToList();

        var data = new StreakData { TotalEntries = allDates.Count };

        if (!allDates.Any()) return data;

        // Calculate Current Streak
        int currentStreak = 0;
        var checkDate = DateTime.Today;
        
        // If today is not in list, check if yesterday is in list (streak active but not updated today)
        if (!allDates.Contains(checkDate))
        {
            if (allDates.Contains(checkDate.AddDays(-1)))
            {
                // Streak is arguably alive if they did it yesterday, usually allowed in apps
                // But calculation starts from most recent entry
                checkDate = allDates.First(); // Should be yesterday or earlier
                // However, strictly speaking, if checkDate (yesterday) is in list, we start counting backwards from it.
                // If the most recent entry is older than yesterday, streak is broken -> 0.
                if (checkDate < DateTime.Today.AddDays(-1))
                {
                    currentStreak = 0;
                }
                else 
                {
                    // Starts counting from this valid entry
                }
            }
            else
            {
                 // No entry today or yesterday -> Streak 0
                 currentStreak = 0;
            }
        }

        // Simple iteration for current streak
        // We iterate backwards from the most recent valid streak date.
        // Actually simpler: Just iterate the sorted dates and check gaps.
        
        var mostRecent = allDates.First();
        bool isStreakActive = (mostRecent == DateTime.Today || mostRecent == DateTime.Today.AddDays(-1));
        
        if (isStreakActive)
        {
            currentStreak = 1;
            for (int i = 0; i < allDates.Count - 1; i++)
            {
                if ((allDates[i] - allDates[i + 1]).TotalDays == 1)
                {
                    currentStreak++;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            currentStreak = 0;
        }

        data.CurrentStreak = currentStreak;

        // Calculate Longest Streak
        int maxStreak = 0;
        int tempStreak = 0;
        if (allDates.Any())
        {
            tempStreak = 1;
            for (int i = 0; i < allDates.Count - 1; i++)
            {
                if ((allDates[i] - allDates[i + 1]).TotalDays == 1)
                {
                    tempStreak++;
                }
                else
                {
                    if (tempStreak > maxStreak) maxStreak = tempStreak;
                    tempStreak = 1;
                }
            }
            if (tempStreak > maxStreak) maxStreak = tempStreak;
        }
        data.LongestStreak = maxStreak;

        // Missed Days (Last 7 days excluding today)
        for (int i = 1; i <= 7; i++)
        {
            var d = DateTime.Today.AddDays(-i);
            if (!allDates.Contains(d))
            {
                data.MissedDays.Add(d);
            }
        }

        return data;
    }

    public async Task<List<MoodStats>> GetMoodDistributionAsync()
    {
        await Init();
        var allJournals = await _db.Database.Table<Journals>().ToListAsync();
        if (!allJournals.Any()) return new List<MoodStats>();

        var total = allJournals.Count;
        var grouped = allJournals.GroupBy(j => j.Mood)
                                 .Select(g => new MoodStats
                                 {
                                     Category = g.Key,
                                     Count = g.Count(),
                                     Percentage = Math.Round((double)g.Count() / total * 100, 1)
                                 })
                                 .OrderByDescending(x => x.Count)
                                 .ToList();
        return grouped;
    }

    public async Task<List<MoodDetailStat>> GetTopMoodDetailsAsync()
    {
        await Init();
        // This requires joining or fetching separately. SQLite-net doesn't do complex joins easily.
        // We'll fetch all journals and all mood details.
        
        var journals = await _db.Database.Table<Journals>().ToListAsync();
        var details = await _db.Database.Table<MoodDetail>().ToListAsync();
        
        // Count usage of Details (Primary + Secondary1 + Secondary2)
        var counts = new Dictionary<int, int>();
        
        foreach (var j in journals)
        {
            if (j.MoodDetailId.HasValue) AddCount(counts, j.MoodDetailId.Value);
            if (j.SecondaryMoodDetailId1.HasValue) AddCount(counts, j.SecondaryMoodDetailId1.Value);
            if (j.SecondaryMoodDetailId2.HasValue) AddCount(counts, j.SecondaryMoodDetailId2.Value);
        }

        var stats = counts.Select(kvp => 
        {
            var d = details.FirstOrDefault(x => x.Id == kvp.Key);
            return d == null ? null : new MoodDetailStat 
            { 
                Name = d.Name, 
                Emoji = d.Emoji, 
                Count = kvp.Value 
            };
        })
        .Where(x => x != null)
        .Select(x => x!)
        .OrderByDescending(x => x.Count)
        .Take(5)
        .ToList();

        return stats;
    }

    private void AddCount(Dictionary<int, int> dict, int id)
    {
        if (!dict.ContainsKey(id)) dict[id] = 0;
        dict[id]++;
    }

    public async Task<List<KeyValuePair<string, int>>> GetTopTagsAsync()
    {
        await Init();
        // Since JournalTags is many-to-many, we fetch distinct tag IDs from JournalTags table?
        // Wait, JournalTags links JournalId and TagId. 
        // We want Top Used Tags.
        
        var journalTags = await _db.Database.Table<JournalTags>().ToListAsync();
        var tags = await _db.Database.Table<Tags>().ToListAsync();
        
        var topTags = journalTags.GroupBy(jt => jt.TagId)
                                 .Select(g => new 
                                 { 
                                     TagId = g.Key, 
                                     Count = g.Count() 
                                 })
                                 .OrderByDescending(x => x.Count)
                                 .Take(5)
                                 .ToList();

        var result = new List<KeyValuePair<string, int>>();
        foreach (var t in topTags)
        {
            var tag = tags.FirstOrDefault(x => x.Id == t.TagId);
            if (tag != null)
            {
                result.Add(new KeyValuePair<string, int>(tag.Name, t.Count));
            }
        }
        return result;
    }

    public async Task<List<KeyValuePair<DateTime, int>>> GetWordCountTrendAsync(int days = 30)
    {
        await Init();
        var cutoff = DateTime.Today.AddDays(-days);
        var journals = await _db.Database.Table<Journals>()
                                .Where(j => j.Date >= cutoff)
                                .ToListAsync();

        return journals.OrderBy(j => j.Date)
                       .Select(j => new KeyValuePair<DateTime, int>(
                           j.Date, 
                           CountWords(j.Content)
                       ))
                       .ToList();
    }

    private int CountWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return 0;
        // Simple word count: strip HTML tags if any?
        // Content might be HTML from editor.
        // Basic approximation: split by space.
        // Ideally strip metadata.
        return content.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
