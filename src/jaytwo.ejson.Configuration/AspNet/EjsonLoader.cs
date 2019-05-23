#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using jaytwo.ejson.Configuration.AspNetCore;
using Newtonsoft.Json;

namespace jaytwo.ejson.Configuration.AspNet
{
    public class EjsonLoader
    {
        private readonly IEJsonCrypto _ejsonCrypto;
        private readonly ConfigurationLoader _configurationLoader;

        public EjsonLoader()
            : this(null)
        {
        }

        internal EjsonLoader(IEJsonCrypto ejsonCrypto)
        {
            _ejsonCrypto = ejsonCrypto ?? new EJsonCrypto();
            _configurationLoader = new ConfigurationLoader();
        }

        public void Load(string filename, bool optional = false, bool overwriteAppSettings = true, bool overwriteConnectionStrings = true)
        {
            if (File.Exists(filename))
            {
                var keyProvider = GetKeyProvider();
                var json = _ejsonCrypto.GetDecryptedJsonFromFile(filename, keyProvider);
                var dictionary = new JavaScriptSerializer().DeserializeObject(json) as Dictionary<string, object>;
                _configurationLoader.Load(dictionary, true, true);
            }
            else
            {
                if (!optional)
                {
                    throw new FileNotFoundException("cannot find ejson", filename);
                }
            }
        }

        private static IPrivateKeyProvider GetKeyProvider()
        {
            var result = new DefaultPrivateKeyProvider();
            result.Add(new AppSettingsPrivateKeyProvider());

            return result;
        }
    }
}
#endif
