using JournalApp.Models;

namespace JournalApp.Services;

public interface IPdfService
{
    byte[] GenerateJournalPdf(List<Journals> journals, string password, string title);
}
