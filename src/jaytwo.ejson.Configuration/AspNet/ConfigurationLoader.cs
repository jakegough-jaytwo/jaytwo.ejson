#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace jaytwo.ejson.Configuration.AspNet
{
    public class ConfigurationLoader
    {
        private NameValueCollection _appSettings;
        private ConnectionStringSettingsCollection _connectionStrings;

        public ConfigurationLoader()
            : this(null, null)
        {
        }

        internal ConfigurationLoader(
            NameValueCollection appSettings,
            ConnectionStringSettingsCollection connectionStrings)
        {
            _appSettings = appSettings ?? ConfigurationManager.AppSettings;
            _connectionStrings = connectionStrings ?? ConfigurationManager.ConnectionStrings;
        }

        public void Load(IDictionary<string, object> values, bool overwriteAppSettings = true, bool overwriteConnectionStrings = true)
        {
            if (overwriteAppSettings)
            {
                var appSettingsValues = GetSection(values, "AppSettings");
                if (appSettingsValues != null)
                {
                    RegisterAppSettings(appSettingsValues);
                }
            }

            if (overwriteConnectionStrings)
            {
                var connectionStringsValues = GetSection(values, "ConnectionStrings");
                if (connectionStringsValues != null)
                {
                    RegisterConnectionStrings(connectionStringsValues);
                }
            }
        }

        private static IDictionary<string, object> GetSection(IDictionary<string, object> values, string key)
        {
            if (values.TryGetValue(key, out object appSettingsValues))
            {
                return appSettingsValues as IDictionary<string, object>;
            }

            return null;
        }

        private void RegisterAppSettings(IDictionary<string, object> appSettings)
        {
            lock (_appSettings)
            {
                foreach (var value in appSettings)
                {
                    var key = value.Key;
                    var valueString = value.Value as string;

                    if (valueString != null)
                    {
                        _appSettings[key] = valueString;
                    }
                }
            }
        }

        private void RegisterConnectionStrings(IDictionary<string, object> connectionStrings)
        {
            lock (_appSettings)
            {
                foreach (var value in connectionStrings)
                {
                    var key = value.Key;
                    var valueString = value.Value as string;

                    if (valueString != null)
                    {
                        SetConnectionString(key, valueString);
                    }
                }
            }
        }

        private void SetConnectionString(string name, string connectionString)
        {
            var connectionStringSettings = _connectionStrings[name];

            if (connectionStringSettings != null)
            {
                lock (connectionStringSettings)
                {
                    connectionStringSettings.ToggleReadOnly(false);
                    connectionStringSettings.ConnectionString = connectionString;
                    connectionStringSettings.ToggleReadOnly(true);
                }
            }
            else
            {
                lock (_connectionStrings)
                {
                    _connectionStrings.ToggleReadOnly(false);
                    _connectionStrings.Add(new ConnectionStringSettings()
                    {
                        Name = name,
                        ConnectionString = connectionString,
                    });
                    _connectionStrings.ToggleReadOnly(true);
                }
            }
        }
    }
}
#endif
