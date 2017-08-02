using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrossbellTranslationTool
{
	static class JsonTextItemFileIO
	{
		static JsonTextItemFileIO()
		{
			var jsonsettings = new JsonSerializerSettings();
			jsonsettings.Formatting = Formatting.Indented;
			jsonsettings.Converters.Add(new EncodingStringJsonConverter());

			JsonSettings = jsonsettings;
		}

		public static List<TextItem> ReadFromFile(String filepath)
		{
			Assert.IsValidString(filepath, nameof(filepath));

			var jsontext = File.ReadAllText(filepath);
			return JsonConvert.DeserializeObject<List<TextItem>>(jsontext, JsonSettings);
		}

		public static void WriteToFile(List<TextItem> list, String filepath)
		{
			Assert.IsNotNull(list, nameof(list));
			Assert.IsValidString(filepath, nameof(filepath));

			var json = JsonConvert.SerializeObject(list, JsonSettings);

			File.Delete(filepath);
			File.WriteAllText(filepath, json);
		}

		static JsonSerializerSettings JsonSettings { get; }
	}
}