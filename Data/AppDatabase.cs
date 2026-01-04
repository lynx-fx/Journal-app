using SQLite;
using JournalApp.Models;

namespace JournalApp.Data
{
    public class AppDatabase
    {
        private SQLiteAsyncConnection _database;

        // Constructor - Initializes database
        public AppDatabase()
        {
        }

        // Initialize database connection and create tables
        async Task Init()
        {
            // If database already exists, don't recreate
            if (_database != null)
                return;

            // Get database location in AppDataDirectory
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "book_management.db");



            Console.WriteLine($"Database Path: {dbPath}");

            //Create SQLiteAsyncConnection
            _database = new SQLiteAsyncConnection(dbPath);

            //Create Tables using CreateTableAsync<T>()
            await _database.CreateTableAsync<Journals>();
            await _database.CreateTableAsync<JournalTags>();
            await _database.CreateTableAsync<Moods>();
            await _database.CreateTableAsync<Tags>();
            

            Console.WriteLine("Database and tables created successfully");
        }
    }
}