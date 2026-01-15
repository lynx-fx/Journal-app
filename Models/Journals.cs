using SQLite;

namespace JournalApp.Models;

public class Journals
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public DateTime Date { get; set; }

    public String Content { get; set; } = String.Empty;

    // Primary mood
    public Moods Mood { get; set; }

    // secondary 1st mood
    public Moods? SecondaryMoodOne { get; set; }

    // secondary 2nd mood
    public Moods? SecondaryMoodTwo { get; set; }

    public int? MoodDetailId { get; set; }
    
    public int? SecondaryMoodDetailId1 { get; set; }
    public int? SecondaryMoodDetailId2 { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }

}