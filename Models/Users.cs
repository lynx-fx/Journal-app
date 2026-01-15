using SQLite;

namespace JournalApp.Models;

public class Users
{
    [PrimaryKey, AutoIncrement]
    public int UserId { get; set; }

    public int Pin { get; set; }
}