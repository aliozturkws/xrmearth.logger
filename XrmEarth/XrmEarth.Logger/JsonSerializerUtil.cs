using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace XrmEarth.Logger
{
    public class JsonSerializerUtil
    {
        public static string Serialize<T>(T input)
        {
            return JsonConvert.SerializeObject(input);
        }

        public static T Deserialize<T>(string output)
        {
            return JsonConvert.DeserializeObject<T>(output);
        }

        public static object Deserialize(string output, Type type)
        {
            return JsonConvert.DeserializeObject(output, type);
        }

        public static void SerializeFile<T>(T input, string filePath)
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented
            };

            using (var stream = new StreamWriter(filePath))
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(stream))
                {
                    jsonSerializer.Serialize(jsonWriter, input);
                }
            }
        }

        public static T DeserializeFile<T>(string filePath)
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented
            };

            using (TextReader reader = File.OpenText(filePath))
            {
                return jsonSerializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }

        public static T DeserializeFile<T>(string filePath, IEnumerable<JsonConverter> customConverters)
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
            };

            foreach (var customConverter in customConverters)
            {
                jsonSerializer.Converters.Add(customConverter);
            }

            using (TextReader reader = File.OpenText(filePath))
            {
                return jsonSerializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }
    }
}
