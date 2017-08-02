using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool
{
	class EncodingStringJsonConverter : JsonConverter
	{
		static EncodingStringJsonConverter()
		{
			var methodinfo = typeof(JsonWriter).GetMethod("InternalWriteValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			InternalWriteValue = (Action<JsonWriter, JsonToken>)Delegate.CreateDelegate(typeof(Action<JsonWriter, JsonToken>), methodinfo);
		}

		public override Boolean CanConvert(Type objectType)
		{
			return objectType == typeof(String);
		}

		public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			var str = (String)value;

			InternalWriteValue(writer, JsonToken.String);

			writer.WriteRaw(EncodedStringUtil.GetStringForJSON(str));
		}

		public override Boolean CanRead => false;

		public override Boolean CanWrite => true;

		static Action<JsonWriter, JsonToken> InternalWriteValue { get; }
	}
}