using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WordProcessor;
using WordProcessor.TextFormatters;
using WordProcessor.TextPresenters;
using WordProcessor.TextProviders;

namespace WordProcessorTests;

[TestFixture]
public class FrequencyDictionaryProcessorTest
{
    private Mock<ITextProvider> _textProviderMock = null!;
    private FrequencyDictionaryTextFormatter _textFormatter = null!;
    private Mock<ITextPresenter> _textPresenterMock = null!;
    private Mock<ILogger>  _loggerMock = null!;
    private FrequencyDictionaryProcessor _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _textProviderMock = new Mock<ITextProvider>();
        _textFormatter = new FrequencyDictionaryTextFormatter();
        _textPresenterMock = new Mock<ITextPresenter>();
        _loggerMock = new Mock<ILogger>();

        _sut = new FrequencyDictionaryProcessor(
            _textProviderMock.Object,
            _textFormatter,
            _textPresenterMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task ProcessAsync_creates_frequency_dictionary_with_expected_delimiter_between_word_and_frequency()
    {
        var inputText = "world world";
        var expectedOutputText = "world,2";

        _textProviderMock
            .Setup(x => x.GetTextAsync())
            .ReturnsAsync(inputText);

        await _sut.ProcessAsync();

        _textPresenterMock.Verify(x => 
                x.PresentAsync(It.Is<string>(y => y == expectedOutputText)), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_creates_frequency_dictionary_with_expected_delimiter_between_word_frequencies()
    {
        var inputText = "Hello world world";
        var expectedOutputText = "world,2\nHello,1";

        _textProviderMock
            .Setup(x => x.GetTextAsync())
            .ReturnsAsync(inputText);

        await _sut.ProcessAsync();

        _textPresenterMock.Verify(x => 
            x.PresentAsync(It.Is<string>(y => y == expectedOutputText)), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_creates_frequency_dictionary_in_case_sensitive_way()
    {
        var inputText = "World world world";
        var expectedOutputText = "world,2\nWorld,1";

        _textProviderMock
            .Setup(x => x.GetTextAsync())
            .ReturnsAsync(inputText);

        await _sut.ProcessAsync();

        _textPresenterMock.Verify(x =>
            x.PresentAsync(It.Is<string>(y => y == expectedOutputText)), Times.Once);
    }

    [TestCase(" ")]
    [TestCase("  ")]
    [TestCase("\r\n")]
    public async Task ProcessAsync_creates_frequency_dictionary_considering_supported_word_separators_in_input_text(
        string wordSeparator)
    {
        var inputText = $"World{wordSeparator}world{wordSeparator}world";
        var expectedOutputText = "world,2\nWorld,1";

        _textProviderMock
            .Setup(x => x.GetTextAsync())
            .ReturnsAsync(inputText);

        await _sut.ProcessAsync();

        _textPresenterMock.Verify(x =>
            x.PresentAsync(It.Is<string>(y => y == expectedOutputText)), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_creates_frequency_dictionary_in_descending_order_of_frequency()
    {
        var inputText = $"New hello world hello world world";
        var expectedOutputText = "world,3\nhello,2\nNew,1";

        _textProviderMock
            .Setup(x => x.GetTextAsync())
            .ReturnsAsync(inputText);

        await _sut.ProcessAsync();

        _textPresenterMock.Verify(x =>
            x.PresentAsync(It.Is<string>(y => y == expectedOutputText)), Times.Once);
    }
}