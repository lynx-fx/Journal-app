using SQLite;

namespace JournalApp.Models;

public class Journals
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public DateTime Date { get; set; }

    public String Content { get; set; } = String.Empty;

    // moods
    public Moods Mood { get; set; }
    public int? PrimaryMoodDetailId { get; set; }
    public int? SecondaryMoodDetailId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
