using JournalApp.Models;

namespace JournalApp.Services;

public interface IJournalsServices
{
    Task Init();
    Task<List<Journals>> GetJournalsAsync();
    Task<List<Journals>> GetJournalsAsync(int skip, int take);
    Task<Journals> GetJournalAsync(DateTime date);
    Task SaveJournalAsync(Journals journal);
    Task DeleteJournalAsync(Journals journal);
}
