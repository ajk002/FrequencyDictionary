namespace WordProcessor.TextFormatters
{
    public interface IFrequencyDictionaryTextFormatter
    {
        string GetText(List<KeyValuePair<string, int>> frequencyDictionary);
    }
}