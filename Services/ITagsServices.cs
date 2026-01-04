using JournalApp.Models;

namespace JournalApp.Services;

public interface ITagsServices
{
    Task Init();
    Task<List<Tags>> GetTagsAsync();
    Task<Tags> GetTagAsync(int id);
    Task<Tags> GetTagByNameAsync(string name);
    Task SaveTagAsync(Tags tag);
    Task DeleteTagAsync(Tags tag);
    Task<List<Tags>> SearchTagsAsync(string query);
}
