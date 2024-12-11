namespace WordProcessor.TextProviders
{
    public interface ITextProvider
    {
        Task<string> GetTextAsync();
    }
}