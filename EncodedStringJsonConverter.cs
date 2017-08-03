using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CrossbellTranslationTool
{
	class EncodedStringJsonConverter : JsonConverter
	{
		static EncodedStringJsonConverter()
		{
			var methodinfo = typeof(JsonWriter).GetMethod("InternalWriteValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			InternalWriteValue = (Action<JsonWriter, JsonToken>)Delegate.CreateDelegate(typeof(Action<JsonWriter, JsonToken>), methodinfo);
		}

		public EncodedStringJsonConverter()
		{
			StringBuilder = new StringBuilder(100);
		}

		public override Boolean CanConvert(Type objectType)
		{
			return objectType == typeof(String);
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(objectType, nameof(objectType));
			Assert.IsNotNull(existingValue, nameof(existingValue));
			Assert.IsNotNull(serializer, nameof(serializer));

			if (reader.TokenType == JsonToken.StartArray)
			{
				StringBuilder.Length = 0;

				var pump = LinqUtil.Pump(() => reader.ReadAsString(), str => str != null);

				foreach (var item in pump) StringBuilder.Append(item);

				return StringBuilder.ToString();
			}

			if (reader.TokenType == JsonToken.String)
			{
				return (String)reader.Value;
			}

			throw new Exception();
		}

		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			var str = (String)value;

			InternalWriteValue(writer, JsonToken.String);

			writer.WriteRaw(EncodedStringUtil.GetStringForJSON(str));
		}

		public override Boolean CanRead => true;

		public override Boolean CanWrite => true;

		StringBuilder StringBuilder { get; }

		static Action<JsonWriter, JsonToken> InternalWriteValue { get; }
	}
}