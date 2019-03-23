using System;
using System.Collections.Generic;
using System.IO;

namespace jaytwo.ejson.Web.Data
{
    public interface IGitDatastore : IDatastore
    {
        IList<string> GetFiles(string pattern);
        void SaveContent(string file, Stream stream, string message);
    }
}
