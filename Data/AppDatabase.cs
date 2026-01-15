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
            await _database.CreateTableAsync<Users>();
            await _database.CreateTableAsync<MoodDetail>();

            await SeedMoods();
            
            Console.WriteLine("Database and tables created successfully");
        }

        private async Task SeedMoods()
        {
            var count = await _database.Table<MoodDetail>().CountAsync();
            if (count == 0)
            {
                var defaults = new List<MoodDetail>
                {
                    // Positive
                    new MoodDetail { Name = "Happy", Emoji = "😊", Category = Moods.Positive },
                    new MoodDetail { Name = "Excited", Emoji = "🤩", Category = Moods.Positive },
                    new MoodDetail { Name = "Grateful", Emoji = "🙏", Category = Moods.Positive },
                    new MoodDetail { Name = "Proud", Emoji = "🦁", Category = Moods.Positive },
                    new MoodDetail { Name = "Relaxed", Emoji = "😌", Category = Moods.Positive },

                    // Neutral
                    new MoodDetail { Name = "Calm", Emoji = "😐", Category = Moods.Neutral },
                    new MoodDetail { Name = "Bored", Emoji = "🥱", Category = Moods.Neutral },
                    new MoodDetail { Name = "Tired", Emoji = "😴", Category = Moods.Neutral },
                    new MoodDetail { Name = "Indifferent", Emoji = "😶", Category = Moods.Neutral },

                    // Negative
                    new MoodDetail { Name = "Sad", Emoji = "😢", Category = Moods.Negative },
                    new MoodDetail { Name = "Angry", Emoji = "😠", Category = Moods.Negative },
                    new MoodDetail { Name = "Anxious", Emoji = "😰", Category = Moods.Negative },
                    new MoodDetail { Name = "Frustrated", Emoji = "😤", Category = Moods.Negative },
                    new MoodDetail { Name = "Lonely", Emoji = "🥀", Category = Moods.Negative }
                };
                
                await _database.InsertAllAsync(defaults);
            }
        }
        
        public SQLiteAsyncConnection Database => _database;
    }
}