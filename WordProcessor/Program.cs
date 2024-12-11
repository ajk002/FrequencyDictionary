using Microsoft.Extensions.Logging;
using WordProcessor.EncodingProviders;
using WordProcessor.TextFormatters;
using WordProcessor.TextPresenters;
using WordProcessor.TextProviders;

namespace WordProcessor;

class Program
{
    static async Task Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger<Program>();

        if (args.Length != 2)
        {
            logger.LogError($"Error! Expected 2 parameters, but found only {args.Length} parameter(s).");
            return;
        }

        string inputFile = args[0];
        string outputFile = args[1];
        logger.LogDebug($"Input file:{inputFile} , Output file:{outputFile}");

        try
        {
            var encodingProvider = new Win1252EncodingProvider();
            var textProvider = new EncodedFileTextProvider(inputFile, encodingProvider, logger);
            var freqDictFormatter = new FrequencyDictionaryTextFormatter();
            var textPresenter = new EncodedFileTextPresenter(outputFile, encodingProvider, logger);

            var freqDictProcessor = new FrequencyDictionaryProcessor(
                textProvider,
                freqDictFormatter,
                textPresenter,
                logger);

            await freqDictProcessor.ProcessAsync();

        }
        catch (Exception ex)
        {
            logger.LogError($"Exception in Main processing. Message = {ex.Message}", ex);
        }

        Console.ReadKey();
    }
}