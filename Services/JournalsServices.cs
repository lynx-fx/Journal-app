using JournalApp.Models;
using SQLite;

namespace JournalApp.Services;

public class JournalsServices : IJournalsServices
{
    private SQLiteAsyncConnection _database;

    public async Task Init()
    {
        try
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Journals>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JournalsServices.Init: {ex}");
            throw;
        }
    }

    public async Task<List<Journals>> GetJournalsAsync()
    {
        await Init();
        return await _database.Table<Journals>().ToListAsync();
    }

    public async Task<Journals> GetJournalAsync(DateTime date)
    {
        await Init();
        // Since sqlite might store ticks or strings, comparing exact DateTime might be tricky if time is involved.
        // Assuming we only care about the Date part.
        // However, SQLite-net-pcl usually stores Ticks or ISO strings.
        // Ideally we should query based on a range or store just the date. 
        // For now, let's assume the Date property is stored as ticks and we query for the start of the day.
        
        // Actually, let's check if the Date stored is normalized to Midnight. The app should ensure that.
        // We will try to find one with the matching Date locally or via query.
        // Querying exactly might be issues with precision.
        // safe approach: get all and filter (for small dataset) or query efficiently.
        // Let's store Date as Ticks, but let's query carefully.
        
        // Better: let's query for the specific date assuming it was saved with time 00:00:00
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _database.Table<Journals>()
                            .Where(j => j.Date >= startOfDay && j.Date <= endOfDay)
                            .FirstOrDefaultAsync();
    }

    public async Task SaveJournalAsync(Journals journal)
    {
        await Init();
        if (journal.Id != 0)
        {
            journal.UpdateAt = DateTime.Now;
            await _database.UpdateAsync(journal);
        }
        else
        {
            journal.CreatedAt = DateTime.Now;
            journal.UpdateAt = DateTime.Now;
            await _database.InsertAsync(journal);
        }
    }

    public async Task DeleteJournalAsync(Journals journal)
    {
        await Init();
        await _database.DeleteAsync(journal);
    }
}
