using SQLITE;

namespace JournalApp.Models;

public class Tags
{
    [PrimaryKey, AutoIncrement]
    public int Id {get; set;}

    [Unique]
    public String Name {get; set;}
}