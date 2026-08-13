namespace tclcnigeria.Services
{
    public interface IAiService
    {
        Task<string> AskAsync(string userQuestion, string contextBlock);
        Task<string> SummarizeSermonAsync(string sermonTitle, string sermonText);
        Task<string> TriagePrayerRequestAsync(string prayerText);
        Task<string> DraftDescriptionAsync(string rawNotes);
    }
}
