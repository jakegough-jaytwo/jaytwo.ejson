#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace jaytwo.ejson.Configuration.AspNet
{
    internal static class ConnectionStringSettingsCollectionExtensions
    {
        public static void ToggleReadOnly(this ConfigurationElementCollection connectionStrings, bool readOnly)
        {
            ToggleReadOnly(connectionStrings, "bReadOnly", readOnly);
        }

        public static void ToggleReadOnly(this ConfigurationElement connectionString, bool readOnly)
        {
            ToggleReadOnly(connectionString, "_bReadOnly", readOnly);
        }

        private static void ToggleReadOnly<T>(T obj, string nonPublicReadOnlyToggleField, bool readOnly)
        {
            var configurationType = obj.GetType().BaseType;
            var readOnlyField = configurationType.GetField(nonPublicReadOnlyToggleField, BindingFlags.Instance | BindingFlags.NonPublic);
            if (readOnlyField == null)
            {
                throw new NotSupportedException($"Could not change write protection on read-only {typeof(T).Name}: non-public field '{nonPublicReadOnlyToggleField}' not found!");
            }

            readOnlyField.SetValue(obj, readOnly);
        }
    }
}
#endif