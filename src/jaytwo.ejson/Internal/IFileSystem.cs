using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson.Internal
{
    internal interface IFileSystem
    {
        void WriteAllText(string path, string contents);
        string ReadAllText(string path);
        bool FileExists(string path);
        long GetFileLength(string path);
    }
}
