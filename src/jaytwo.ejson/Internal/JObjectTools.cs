using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace jaytwo.ejson.Internal
{
    internal static class JObjectTools
    {
        public static JObject GetJObject(Stream stream)
        {
            var reader = new StreamReader(stream);
            return GetJObject(reader);
        }

        public static JObject GetJObject(TextReader reader)
        {
            var jsonReader = new JsonTextReader(reader);
            return JObject.Load(jsonReader);
        }

        public static string NormalizeJson(string json)
        {
            return GetJson(GetJObject(json));
        }

        public static JObject GetJObject(string json)
        {
            var reader = new StringReader(json);
            return GetJObject(reader);
        }

        public static void Write(JObject jObject, Stream stream)
        {
            stream.Position = 0;
            var writer = new StreamWriter(stream);
            Write(jObject, writer);
            stream.Flush();
            stream.SetLength(stream.Position);
        }

        public static void Write(JObject jObject, TextWriter writer)
        {
            var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented,
                IndentChar = ' ',
                Indentation = 2
            };

            jObject.WriteTo(jsonWriter);
            writer.Flush();
        }

        public static string GetJson(JObject jObject)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                Write(jObject, writer);
            }

            return stringBuilder.ToString();
        }
    }
}
