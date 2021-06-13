using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Helpers.Extensions
{
    public static class PGPExtensions
    {
        public static string GetFormattedHex(this string instance, bool shortForm = false)
        {
            var data = instance.ToCharArray();
            var builder = new StringBuilder();

            var startIndex = 0;
            if (shortForm)
            {
                startIndex = data.Length - 8;
            }

            for (var i = startIndex; i < data.Length; i++)
            {
                builder.Append(data[i]);
                if ((i + 1) % 4 == 0)
                {
                    builder.Append(' ');
                }
            }

            return builder.ToString().Trim().ToUpper();
        }

        public static string GetKeyIdHex(this long instance, bool shortForm = false)
        {
            return instance.ToString("X").GetFormattedHex(shortForm);
        }

        public static Stream GetStream(this string content, Encoding encoding = null)
        {
            var stream = new MemoryStream();
            var writer = encoding is not null ? new StreamWriter(stream, encoding) : new StreamWriter(stream);

            writer.Write(content);
            writer.Flush();

            stream.Position = 0;
            return stream;
        }

        public static async Task<Stream> GetStreamAsync(this string content, Encoding encoding = null)
        {
            var stream = new MemoryStream();
            var writer = encoding is not null ? new StreamWriter(stream, encoding) : new StreamWriter(stream);

            await writer.WriteAsync(content);
            await writer.FlushAsync();

            stream.Position = 0;
            return stream;
        }

        public static string GetString(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static async Task<string> GetStringAsync(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
