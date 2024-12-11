using System.Text;
using NUnit.Framework;
using WordProcessor.EncodingProviders;

namespace WordProcessorTests;

[TestFixture]
public class Win1252EncodingProviderTests
{
    [Test]
    public void GetsWindows1252Encoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var expectedEncoding = Encoding.GetEncoding(1252);

        var encodingProvider = new Win1252EncodingProvider();

        Assert.That(encodingProvider.Encoding, Is.EqualTo(expectedEncoding));
    }
}