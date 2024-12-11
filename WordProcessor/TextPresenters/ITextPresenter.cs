namespace WordProcessor.TextPresenters
{
    public interface ITextPresenter
    {
        Task PresentAsync(string text);
    }
}