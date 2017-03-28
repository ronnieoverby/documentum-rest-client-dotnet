using System.IO;

namespace Emc.Documentum.Rest
{
    static class Extensions
    {
        public static bool IsNullOrWhiteSpace(this string s) =>
            string.IsNullOrEmpty(s?.Trim());

        public static void CopyTo(this Stream source, Stream destination)
        {
            var buffer = new byte[81920];
            var read = 0;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, read);
        }
    }
}
