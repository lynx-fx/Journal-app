using JournalApp.Models;
using SQLite;
using JournalApp.Data;

namespace JournalApp.Services;

public class TagsServices : ITagsServices
{
    private readonly AppDatabase _appDatabase;

    public TagsServices(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task Init()
    {
        await _appDatabase.Init();
        
        if (await _appDatabase.Database.Table<Tags>().CountAsync() == 0)
        {
            var defaultTags = new List<Tags>
            {
                new Tags { Name = "Work" },
                new Tags { Name = "Personal" },
                new Tags { Name = "Health" },
                new Tags { Name = "Idea" },
                new Tags { Name = "Gratitude" }
            };
            await _appDatabase.Database.InsertAllAsync(defaultTags);
        }
    }

    public async Task<List<Tags>> GetTagsAsync()
    {
        await Init();
        return await _appDatabase.Database.Table<Tags>().ToListAsync();
    }

    public async Task<Tags> GetTagAsync(int id)
    {
        await Init();
        return await _appDatabase.Database.Table<Tags>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Tags> GetTagByNameAsync(string name)
    {
        await Init();
        return await _appDatabase.Database.Table<Tags>().Where(t => t.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task SaveTagAsync(Tags tag)
    {
        await Init();
        if (tag.Id != 0)
        {
            await _appDatabase.Database.UpdateAsync(tag);
        }
        else
        {
            await _appDatabase.Database.InsertAsync(tag);
        }
    }

    public async Task DeleteTagAsync(Tags tag)
    {
        await Init();
        await _appDatabase.Database.DeleteAsync(tag);
    }

    public async Task<List<Tags>> SearchTagsAsync(string query)
    {
        await Init();
        if (string.IsNullOrWhiteSpace(query))
            return new List<Tags>();
            
        return await _appDatabase.Database.Table<Tags>()
            .Where(t => t.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync();
    }
}
