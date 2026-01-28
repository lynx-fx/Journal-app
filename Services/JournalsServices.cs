using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Services;

public class JournalsServices : IJournalsServices
{
    private readonly AppDatabase _appDatabase;

    public JournalsServices(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task Init()
    {
        await _appDatabase.Init();
    }

    public async Task<List<Journals>> GetJournalsAsync()
    {
        await Init();
        return await _appDatabase
            .Database.Table<Journals>()
            .OrderByDescending(j => j.Date)
            .ToListAsync();
    }

    public async Task<List<Journals>> GetJournalsAsync(int skip, int take)
    {
        await Init();
        return await _appDatabase
            .Database.Table<Journals>()
            .OrderByDescending(j => j.Date)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Journals> GetJournalAsync(DateTime date)
    {
        await Init();
        // LOGIC: Calculate start and end of the specific date to capture any time on that day.
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _appDatabase
            .Database.Table<Journals>()
            .Where(j => j.Date >= startOfDay && j.Date <= endOfDay)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Journals>> SearchJournalsAsync(string query, string filterType)
    {
        await Init();

        string lowerQuery = query.ToLower();
        string fuzzyQuery = $"%{lowerQuery}%";

        if (filterType == "Content")
        {
            // Use Raw SQL for reliable case-insensitive LIKE
            return await _appDatabase.Database.QueryAsync<Journals>(
                "SELECT * FROM Journals WHERE lower(Content) LIKE ? OR lower(Title) LIKE ? ORDER BY Date DESC", 
                fuzzyQuery, fuzzyQuery);
        }
        else if (filterType == "Mood")
        {
            var matchingJournals = new List<Journals>();

            // 1. Enum Search (Primary Category)
            // Just check exact match on the Enum string representation first (e.g. "Positive")
            if (Enum.TryParse<Moods>(query, true, out var mood))
            {
                var enumMatches = await _appDatabase
                    .Database.Table<Journals>()
                    .Where(j => j.Mood == mood)
                    .OrderByDescending(j => j.Date)
                    .ToListAsync();
                matchingJournals.AddRange(enumMatches);
            }

            // 2. Detail Search (Secondary Moods) using SQL for join/subselect logic implicitly by ID
            // Find detail IDs first
            var matchingDetails = await _appDatabase.Database.QueryAsync<MoodDetail>(
                "SELECT * FROM MoodDetail WHERE lower(Name) LIKE ?", fuzzyQuery);

            if (matchingDetails.Any())
            {
                var detailIds = matchingDetails.Select(m => m.Id).ToList();
                var idsStr = string.Join(",", detailIds);
                
                var detailMatches = await _appDatabase.Database.QueryAsync<Journals>(
                    $"SELECT * FROM Journals WHERE PrimaryMoodDetailId IN ({idsStr}) OR SecondaryMoodDetailId IN ({idsStr}) ORDER BY Date DESC");
                
                matchingJournals.AddRange(detailMatches);
            }

            return matchingJournals
                .GroupBy(j => j.Id)
                .Select(g => g.First())
                .OrderByDescending(j => j.Date)
                .ToList();
        }
        else if (filterType == "Tags")
        {
            // 1. Find Tag Ids
            var tags = await _appDatabase.Database.QueryAsync<Tags>(
                "SELECT * FROM Tags WHERE lower(Name) LIKE ?", fuzzyQuery);

            if (!tags.Any())
                return new List<Journals>();

            var tagIds = tags.Select(t => t.Id).ToList();
            var tagIdsString = string.Join(",", tagIds);

            // 2. Find Journal Ids from JournalTags
            var querySql = $"SELECT * FROM JournalTags WHERE TagId IN ({tagIdsString})";
            var journalTags = await _appDatabase.Database.QueryAsync<JournalTags>(querySql);

            var journalIds = journalTags.Select(jt => jt.JournalEntryId).Distinct().ToList();

            if (!journalIds.Any())
                return new List<Journals>();

            // 3. Fetch Journals
            var journalIdsString = string.Join(",", journalIds);
            var journalsQuery = $"SELECT * FROM Journals WHERE Id IN ({journalIdsString}) ORDER BY Date DESC";
            return await _appDatabase.Database.QueryAsync<Journals>(journalsQuery);
        }
        else if (filterType == "Date")
        {
            // Same date logic as before
            if (DateTime.TryParseExact(query, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var isoDate))
            {
                var start = isoDate.Date;
                var end = isoDate.Date.AddDays(1).AddTicks(-1);
                return await _appDatabase
                    .Database.Table<Journals>()
                    .Where(j => j.Date >= start && j.Date <= end)
                    .ToListAsync();
            }
            else if (DateTime.TryParse(query, out var date))
            {
                var start = date.Date;
                var end = date.Date.AddDays(1).AddTicks(-1);
                return await _appDatabase
                    .Database.Table<Journals>()
                    .Where(j => j.Date >= start && j.Date <= end)
                    .ToListAsync();
            }
        }

        return new List<Journals>();
    }

    public async Task SaveJournalAsync(Journals journal)
    {
        await Init();
        if (journal.Id != 0)
        {
            journal.UpdateAt = DateTime.Now;
            await _appDatabase.Database.UpdateAsync(journal);
        }
        else
        {
            journal.CreatedAt = DateTime.Now;
            journal.UpdateAt = DateTime.Now;
            await _appDatabase.Database.InsertAsync(journal);
        }
    }

    public async Task DeleteJournalAsync(Journals journal)
    {
        await Init();
        await _appDatabase.Database.DeleteAsync(journal);
    }
}
