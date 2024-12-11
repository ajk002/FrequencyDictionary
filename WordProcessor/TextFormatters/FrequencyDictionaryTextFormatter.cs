namespace WordProcessor.TextFormatters
{
    public class FrequencyDictionaryTextFormatter : IFrequencyDictionaryTextFormatter
    {
        public string GetText(List<KeyValuePair<string, int>> frequencyDictionary)
        {
            return string.Join("\n", frequencyDictionary.Select(x => $"{x.Key},{x.Value}"));
        }
    }
}