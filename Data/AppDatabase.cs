using SQLite;
using JournalApp.Models;

namespace JournalApp.Data
{
    public class AppDatabase
    {
        private SQLiteAsyncConnection _database;

        public AppDatabase()
        {
        }

        public async Task Init()
        {
            if (_database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal_active.db3");

            Console.WriteLine($"Database Path: {dbPath}");

            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Journals>();
            await _database.CreateTableAsync<JournalTags>();
            await _database.CreateTableAsync<Tags>();
            
            Console.WriteLine("Database and tables created successfully");
        }
        
        public SQLiteAsyncConnection Database => _database;
    }
}