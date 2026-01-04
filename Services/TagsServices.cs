using JournalApp.Models;
using SQLite;

namespace JournalApp.Services;

public class TagsServices : ITagsServices
{
    private SQLiteAsyncConnection? _database;

    public async Task Init()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Tags>();

        // Seed default tags if empty
        if (await _database.Table<Tags>().CountAsync() == 0)
        {
            var defaultTags = new List<Tags>
            {
                new Tags { Name = "Work" },
                new Tags { Name = "Personal" },
                new Tags { Name = "Health" },
                new Tags { Name = "Idea" },
                new Tags { Name = "Gratitude" }
            };
            await _database.InsertAllAsync(defaultTags);
        }
    }

    public async Task<List<Tags>> GetTagsAsync()
    {
        await Init();
        return await _database!.Table<Tags>().ToListAsync();
    }

    public async Task<Tags> GetTagAsync(int id)
    {
        await Init();
        return await _database!.Table<Tags>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Tags> GetTagByNameAsync(string name)
    {
        await Init();
        return await _database!.Table<Tags>().Where(t => t.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task SaveTagAsync(Tags tag)
    {
        await Init();
        if (tag.Id != 0)
        {
            await _database!.UpdateAsync(tag);
        }
        else
        {
            await _database!.InsertAsync(tag);
        }
    }

    public async Task DeleteTagAsync(Tags tag)
    {
        await Init();
        await _database!.DeleteAsync(tag);
    }

    public async Task<List<Tags>> SearchTagsAsync(string query)
    {
        await Init();
        if (string.IsNullOrWhiteSpace(query))
            return new List<Tags>();
            
        return await _database!.Table<Tags>()
            .Where(t => t.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync();
    }
}
