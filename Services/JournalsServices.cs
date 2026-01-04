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
