using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Services;

public class UsersService : IUsersService
{
    private readonly AppDatabase _db;

    public UsersService(AppDatabase db)
    {
        _db = db;
    }

    private async Task Init()
    {
        await _db.Init();
    }

    public async Task<bool> HasPinAsync()
    {
        await Init();
        var count = await _db.Database.Table<Users>().CountAsync();
        return count > 0;
    }

    public async Task<bool> ValidatePinAsync(string pin)
    {
        await Init();
        // Since we are storing as int in model but handling as string logic often
        // let's stick to the model which says 'int Pin'. 
        // Although PINs with leading zeros might be an issue with 'int', 
        // the user model defined it as int. I will parse it.
        
        if (int.TryParse(pin, out int pinVal))
        {
            var user = await _db.Database.Table<Users>().FirstOrDefaultAsync();
            return user != null && user.Pin == pinVal;
        }
        return false;
    }

    public async Task SavePinAsync(string pin)
    {
        await Init();
        if (int.TryParse(pin, out int pinVal))
        {
            var user = await _db.Database.Table<Users>().FirstOrDefaultAsync();
            if (user == null)
            {
                user = new Users { Pin = pinVal };
                await _db.Database.InsertAsync(user);
            }
            else
            {
                user.Pin = pinVal;
                await _db.Database.UpdateAsync(user);
            }
        }
    }
}
