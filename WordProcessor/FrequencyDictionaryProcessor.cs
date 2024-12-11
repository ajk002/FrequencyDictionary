using Microsoft.Extensions.Logging;
using WordProcessor.TextFormatters;
using WordProcessor.TextPresenters;
using WordProcessor.TextProviders;

namespace WordProcessor
{
    public class FrequencyDictionaryProcessor(
        ITextProvider textProvider,
        IFrequencyDictionaryTextFormatter frequencyDictionaryFormatter,
        ITextPresenter textPresenter,
        ILogger logger)
    {
        private readonly string[] _wordDelimiters = [" ", Environment.NewLine];

        public async Task ProcessAsync()
        {
            try
            {
                var inputText = await textProvider.GetTextAsync();

                if (string.IsNullOrEmpty(inputText))
                {
                    logger.LogInformation("Input text is empty, so no further processing");
                    return;
                }

                var frequencyDictionary = CreateFrequencyDictionaryFromText(inputText);

                OrderByDescendingFrequency(frequencyDictionary);

                var outputText = frequencyDictionaryFormatter.GetText(frequencyDictionary);

                await textPresenter.PresentAsync(outputText);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception in frequency dictionary processing. Message = {ex.Message}", ex);
            }
        }

        private void OrderByDescendingFrequency(List<KeyValuePair<string, int>> frequencyDictionary)
        {
            frequencyDictionary.Sort((a,b) => b.Value - a.Value);
        }

        private List<KeyValuePair<string, int>> CreateFrequencyDictionaryFromText(string text)
        {
            var words = text.Split(_wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
           
            var frequencyDictionary = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (!frequencyDictionary.TryAdd(word, 1))
                    frequencyDictionary[word]++;
            }

            return frequencyDictionary.ToList();
        }
    }
}
