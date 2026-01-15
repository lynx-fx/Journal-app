using JournalApp.Models;

namespace JournalApp.Services;

public interface IMoodsService
{
    Task<List<MoodDetail>> GetMoodsByCategoryAsync(Moods category);
    Task<List<MoodDetail>> GetAllMoodsAsync();
    Task AddCustomMoodAsync(MoodDetail mood);
    Task<MoodDetail?> GetMoodByIdAsync(int id);
}
