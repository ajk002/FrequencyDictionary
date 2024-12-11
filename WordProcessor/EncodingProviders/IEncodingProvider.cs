using System.Text;

namespace WordProcessor.EncodingProviders
{
    public interface IEncodingProvider
    {
        Encoding Encoding { get; }
    }
}