using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Services;

public class MoodsService : IMoodsService
{
    private readonly AppDatabase _db;

    public MoodsService(AppDatabase db)
    {
        _db = db;
    }

    private async Task Init()
    {
        await _db.Init();
    }

    public async Task<List<MoodDetail>> GetMoodsByCategoryAsync(Moods category)
    {
        await Init();
        return await _db.Database.Table<MoodDetail>()
            .Where(m => m.Category == category)
            .ToListAsync();
    }

    public async Task<List<MoodDetail>> GetAllMoodsAsync()
    {
        await Init();
        return await _db.Database.Table<MoodDetail>().ToListAsync();
    }

    public async Task AddCustomMoodAsync(MoodDetail mood)
    {
        await Init();
        await _db.Database.InsertAsync(mood);
    }
    
    public async Task<MoodDetail?> GetMoodByIdAsync(int id)
    {
        await Init();
        return await _db.Database.Table<MoodDetail>().FirstOrDefaultAsync(m => m.Id == id);
    }
}
