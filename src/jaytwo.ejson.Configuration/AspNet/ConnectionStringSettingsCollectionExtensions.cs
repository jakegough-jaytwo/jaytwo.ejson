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
            ToggleReadOnlyCommon(connectionStrings, "bReadOnly", readOnly);
        }

        public static void ToggleReadOnly(this ConfigurationElement connectionString, bool readOnly)
        {
            ToggleReadOnlyCommon(connectionString, "_bReadOnly", readOnly);
        }

        private static void ToggleReadOnlyCommon(this ConfigurationElement obj, string nonPublicReadOnlyToggleField, bool readOnly)
        {
            var objType = obj.GetType();
            var configurationType = objType.BaseType;
            var readOnlyField = configurationType.GetField(nonPublicReadOnlyToggleField, BindingFlags.Instance | BindingFlags.NonPublic);
            if (readOnlyField == null)
            {
                throw new NotSupportedException($"Could not change write protection on read-only {objType.Name}: non-public field '{nonPublicReadOnlyToggleField}' not found!");
            }

            readOnlyField.SetValue(obj, readOnly);
        }
    }
}
#endif
