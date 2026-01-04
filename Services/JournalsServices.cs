using JournalApp.Models;
using SQLite;
using JournalApp.Data;

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
        return await _appDatabase.Database.Table<Journals>().OrderByDescending(j => j.Date).ToListAsync();
    }

    public async Task<List<Journals>> GetJournalsAsync(int skip, int take)
    {
        await Init();
        return await _appDatabase.Database.Table<Journals>()
                              .OrderByDescending(j => j.Date)
                              .Skip(skip)
                              .Take(take)
                              .ToListAsync();
    }

    public async Task<Journals> GetJournalAsync(DateTime date)
    {
        await Init();
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _appDatabase.Database.Table<Journals>()
                            .Where(j => j.Date >= startOfDay && j.Date <= endOfDay)
                            .FirstOrDefaultAsync();
    }

    public async Task<List<Journals>> SearchJournalsAsync(string query, string filterType)
    {
        await Init();
        
        string lowerQuery = query.ToLower();

        if (filterType == "Content")
        {
            return await _appDatabase.Database.Table<Journals>()
                         .Where(j => j.Content.Contains(query))
                         .OrderByDescending(j => j.Date)
                         .ToListAsync();
        }
        else if (filterType == "Mood")
        {
            // flexible parsing
            if (Enum.TryParse<Moods>(query, true, out var mood))
            {
                return await _appDatabase.Database.Table<Journals>()
                             .Where(j => j.Mood == mood)
                             .OrderByDescending(j => j.Date)
                             .ToListAsync();
            }
            return new List<Journals>();
        }
        else if (filterType == "Tags")
        {
            // 1. Find Tag Ids matching name
            var tags = await _appDatabase.Database.Table<Tags>()
                             .Where(t => t.Name.Contains(query))
                             .ToListAsync();
            
            if (!tags.Any()) return new List<Journals>();

            var tagIds = tags.Select(t => t.Id).ToList();

            // 2. Find Journal Ids from JournalTags
            // SQLite-net-pcl doesn't support complex Contains with lists well in linq-to-sql sometimes,
            // but let's try standard approach or use raw query if needed.
            // Raw query is often safer for "IN" clauses with list of ints.
            
            var tagIdsString = string.Join(",", tagIds);
            var querySql = $"SELECT * FROM JournalTags WHERE TagId IN ({tagIdsString})";
            var journalTags = await _appDatabase.Database.QueryAsync<JournalTags>(querySql);
            
            var journalIds = journalTags.Select(jt => jt.JournalEntryId).Distinct().ToList();

            if (!journalIds.Any()) return new List<Journals>();

            // 3. Fetch Journals
             // Again, "IN" clause with raw query or multiple fetches.
             var journalIdsString = string.Join(",", journalIds);
             var journalsQuery = $"SELECT * FROM Journals WHERE Id IN ({journalIdsString}) ORDER BY Date DESC";
             return await _appDatabase.Database.QueryAsync<Journals>(journalsQuery);
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
