namespace JournalApp.Services;

public interface IUsersService
{
    Task<bool> HasPinAsync();
    Task<bool> ValidatePinAsync(string pin);
    Task SavePinAsync(string pin);
}
