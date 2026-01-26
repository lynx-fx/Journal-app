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

        if (filterType == "Content")
        {
            return await _appDatabase
                .Database.Table<Journals>()
                .Where(j => j.Content.Contains(query) || j.Title.Contains(query))
                .OrderByDescending(j => j.Date)
                .ToListAsync();
        }
        else if (filterType == "Mood")
        {
            // flexible parsing
            if (Enum.TryParse<Moods>(query, true, out var mood))
            {
                return await _appDatabase
                    .Database.Table<Journals>()
                    .Where(j => j.Mood == mood)
                    .OrderByDescending(j => j.Date)
                    .ToListAsync();
            }
            return new List<Journals>();
        }
        else if (filterType == "Tags")
        {
            // LOGIC: Tag Search is a 3-step process because of the Many-to-Many relationship (Journal <-> JournalTags <-> Tags).
            // 1. Find Tag Ids matching name
            var tags = await _appDatabase
                .Database.Table<Tags>()
                .Where(t => t.Name.Contains(query))
                .ToListAsync();

            if (!tags.Any())
                return new List<Journals>();

            var tagIds = tags.Select(t => t.Id).ToList();

            // 2. Find Journal Ids from JournalTags
            // LOGIC: Using Raw SQL for 'IN' clause because LINQ-to-SQL in SQLite-net-pcl has limitations with List.Contains() for complex queries.
            // This ensures we get all JournalEntryIds that have ANY of the matching TagIds.
            var tagIdsString = string.Join(",", tagIds);
            var querySql = $"SELECT * FROM JournalTags WHERE TagId IN ({tagIdsString})";
            var journalTags = await _appDatabase.Database.QueryAsync<JournalTags>(querySql);

            var journalIds = journalTags.Select(jt => jt.JournalEntryId).Distinct().ToList();

            if (!journalIds.Any())
                return new List<Journals>();

            // 3. Fetch Journals
            // LOGIC: Final fetch using Raw SQL again for the IN clause on Journal Ids.
            var journalIdsString = string.Join(",", journalIds);
            var journalsQuery =
                $"SELECT * FROM Journals WHERE Id IN ({journalIdsString}) ORDER BY Date DESC";
            return await _appDatabase.Database.QueryAsync<Journals>(journalsQuery);
        }
        else if (filterType == "Date")
        {
            if (DateTime.TryParse(query, out var date))
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
