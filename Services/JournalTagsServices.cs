using JournalApp.Models;
using SQLite;

namespace JournalApp.Services;

public class JournalTagsServices : IJournalTagsServices
{
    private SQLiteAsyncConnection? _database;

    // Use a separate method or dependency injection to access Tags table if needed,
    // but here we might just need joins or standard queries.
    // For simplicity, we'll assume we can query Tags table directly in the same DB connection context 
    // or we might need to handle connection sharing. 
    // SQLiteAsyncConnection is thread safe and we are using the same file.

    public async Task Init()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<JournalTags>();
    }

    public async Task AddTagToJournalAsync(int journalId, int tagId)
    {
        await Init();
        // Check if exists
        var exists = await _database!.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId && jt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (exists == null)
        {
            await _database.InsertAsync(new JournalTags { JournalEntryId = journalId, TagId = tagId });
        }
    }

    public async Task RemoveTagFromJournalAsync(int journalId, int tagId)
    {
        await Init();
        var link = await _database!.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId && jt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (link != null)
        {
            await _database.DeleteAsync(link);
        }
    }

    public async Task RemoveAllTagsFromJournalAsync(int journalId)
    {
        await Init();
        // Delete all links for this journal
        var query = "DELETE FROM JournalTags WHERE JournalEntryId = ?";
        await _database!.ExecuteAsync(query, journalId);
    }

    public async Task<List<Tags>> GetTagsForJournalAsync(int journalId)
    {
        await Init();
        
        // Manual join
        var tagIds = await _database!.Table<JournalTags>()
            .Where(jt => jt.JournalEntryId == journalId)
            .ToListAsync();
            
        var tags = new List<Tags>();
        foreach (var link in tagIds)
        {
             var tag = await _database.Table<Tags>().Where(t => t.Id == link.TagId).FirstOrDefaultAsync();
             if(tag != null) tags.Add(tag);
        }
        return tags;
    }
}
