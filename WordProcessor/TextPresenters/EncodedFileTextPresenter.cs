using Microsoft.Extensions.Logging;
using WordProcessor.EncodingProviders;

namespace WordProcessor.TextPresenters
{
    public class EncodedFileTextPresenter(
        string filePath,
        IEncodingProvider encodingProvider,
        ILogger logger) : ITextPresenter
    {
        public async Task PresentAsync(string text)
        {
            try
            {
                await File.WriteAllTextAsync(
                    filePath,
                    text,
                    encodingProvider.Encoding);

                logger.LogInformation($"Successfully wrote text to file: {filePath}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception writing text to file: {filePath}.", ex);
            }
        }
    }
}