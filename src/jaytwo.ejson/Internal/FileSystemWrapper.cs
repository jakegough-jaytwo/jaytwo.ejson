using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson.Internal
{
    public class FileSystemWrapper : IFileSystem
    {
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public string CombinePath(params string[] paths) => Path.Combine(paths);
        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
        public string ReadAllText(string path) => File.ReadAllText(path);
        public bool FileExists(string path) => File.Exists(path);
        public long GetFileLength(string path) => new FileInfo(path).Length;
    }
}
