using JournalApp.Models;

namespace JournalApp.Services;

public interface IJournalTagsServices
{
    Task Init();
    Task AddTagToJournalAsync(int journalId, int tagId);
    Task RemoveTagFromJournalAsync(int journalId, int tagId);
    Task<List<Tags>> GetTagsForJournalAsync(int journalId);
    Task RemoveAllTagsFromJournalAsync(int journalId);
}
