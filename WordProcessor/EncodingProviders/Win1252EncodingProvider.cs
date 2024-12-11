using System.Text;

namespace WordProcessor.EncodingProviders
{
    public class Win1252EncodingProvider : IEncodingProvider
    {
        static Win1252EncodingProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public Encoding Encoding => Encoding.GetEncoding(1252);
    }
}