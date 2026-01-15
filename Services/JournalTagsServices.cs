using JournalApp.Data;
using JournalApp.Models;
using SQLite;

namespace JournalApp.Services;

public class JournalTagsServices : IJournalTagsServices
{
    private readonly AppDatabase _appDatabase;

    public JournalTagsServices(AppDatabase appDatabase)
    {
        _appDatabase = appDatabase;
    }

    public async Task Init()
    {
        await _appDatabase.Init();
    }

    public async Task AddTagToJournalAsync(int journalId, int tagId)
    {
        await Init();
        // Check if exists
        var exists = await _appDatabase.Database.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId && jt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (exists == null)
        {
            await _appDatabase.Database.InsertAsync(new JournalTags { JournalEntryId = journalId, TagId = tagId });
        }
    }

    public async Task RemoveTagFromJournalAsync(int journalId, int tagId)
    {
        await Init();
        var link = await _appDatabase.Database.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId && jt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (link != null)
        {
            await _appDatabase.Database.DeleteAsync(link);
        }
    }

    public async Task RemoveAllTagsFromJournalAsync(int journalId)
    {
        await Init();
        // Delete all links for this journal
        var query = "DELETE FROM JournalTags WHERE JournalEntryId = ?";
        await _appDatabase.Database.ExecuteAsync(query, journalId);
    }

    public async Task<List<Tags>> GetTagsForJournalAsync(int journalId)
    {
        await Init();
        
        // Manual join
        var tagIds = await _appDatabase.Database.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId)
            .ToListAsync();
            
        var tags = new List<Tags>();
        foreach (var link in tagIds)
        {
             var tag = await _appDatabase.Database.Table<Tags>().Where(t => t.Id == link.TagId).FirstOrDefaultAsync();
             if(tag != null) tags.Add(tag);
        }
        return tags;
    }
}
