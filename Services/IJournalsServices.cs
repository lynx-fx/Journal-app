using JournalApp.Models;

namespace JournalApp.Services;

public interface IJournalsServices
{
    Task Init();
    Task<List<Journals>> GetJournalsAsync();
    Task<Journals> GetJournalAsync(DateTime date);
    Task SaveJournalAsync(Journals journal);
    Task DeleteJournalAsync(Journals journal);
}
