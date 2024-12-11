using Microsoft.Extensions.Logging;
using WordProcessor.EncodingProviders;

namespace WordProcessor.TextProviders
{
    public class EncodedFileTextProvider(
        string filePath,
        IEncodingProvider encodingProvider,
        ILogger logger) : ITextProvider
    {
        public async Task<string> GetTextAsync()
        {
            string text = string.Empty;

            try
            {
                var bytes = await File.ReadAllBytesAsync(filePath);
                text = encodingProvider.Encoding.GetString(bytes);

                logger.LogInformation($"Successfully read content from file {filePath}");
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}", ex);
            }

            return text;
        }
    }
}