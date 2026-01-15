using SQLite;

namespace JournalApp.Models;

public class MoodDetail
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Emoji { get; set; } = string.Empty;
    
    // Foreign Key link to the main category enum
    public Moods Category { get; set; }
}
