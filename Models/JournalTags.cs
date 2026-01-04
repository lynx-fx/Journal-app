using SQLite;

namespace JournalApp.Models;

public class JournalTags
{
    [Indexed]
    public int JournalEntryId { get; set; }

    [Indexed]
    public int TagId { get; set; }
}