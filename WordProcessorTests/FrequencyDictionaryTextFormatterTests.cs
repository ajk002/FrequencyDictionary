using NUnit.Framework;
using WordProcessor.TextFormatters;

namespace WordProcessorTests;

[TestFixture]
public class FrequencyDictionaryTextFormatterTests
{
    [Test]
    public void GetText_formats_by_placing_correct_separator_between_word_and_frequency()
    {
        const string expectedFormattedText = "Hello,10";
        var freqDictionary = new List<KeyValuePair<string, int>>
        {
            new("Hello", 10 )
        };

        var textFormatter = new FrequencyDictionaryTextFormatter();

        var formattedFrequency = textFormatter.GetText(freqDictionary);
        
        Assert.That(formattedFrequency, Is.EqualTo(expectedFormattedText));
    }

    [Test]
    public void GetText_formats_by_placing_newline_char_between_word_frequencies()
    {
        const string expectedFormattedText = "Hello,10\nWorld,5";
        var freqDictionary = new List<KeyValuePair<string, int>>
        {
            new("Hello", 10 ), new("World", 5 )
        };

        var textFormatter = new FrequencyDictionaryTextFormatter();

        var formattedFrequency = textFormatter.GetText(freqDictionary);

        Assert.That(formattedFrequency, Is.EqualTo(expectedFormattedText));
    }
}