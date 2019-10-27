using System;

namespace jaytwo.ejson.Internal
{
    internal static class IFileSystemExtensions
    {
        public static void WriteAllTextWithFinalNewLine(this IFileSystem fileSystem, string path, string contents)
        {
            var contentsWithNewLine = contents.TrimEnd() + Environment.NewLine;
            fileSystem.WriteAllText(path, contentsWithNewLine);
        }
    }
}
