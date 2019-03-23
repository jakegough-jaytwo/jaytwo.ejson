using System;
using System.IO;

namespace jaytwo.ejson.Web.Data
{
    public interface IDatastore
    {
        Stream ReadContent(string key);
        void SaveContent(string key, Stream stream);
    }
}
