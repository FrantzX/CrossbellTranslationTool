using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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

			var tokens = TokenizeString(str);
			if (tokens.Count > 1)
			{
				writer.WriteStartArray();

				foreach (var token in tokens)
				{
					InternalWriteValue(writer, JsonToken.String);

					writer.WriteRaw(EncodedStringUtil.GetStringForJSON(token));
				}

				writer.WriteEndArray();
			}
			else
			{
				InternalWriteValue(writer, JsonToken.String);

				writer.WriteRaw(EncodedStringUtil.GetStringForJSON(str));
			}
		}

		List<String> TokenizeString(String value)
		{
			Assert.IsNotNull(value, nameof(value));

			StringBuilder.Length = 0;

			var tokens = new List<String>();

			void AddToken()
			{
				if (StringBuilder.Length > 0)
				{
					tokens.Add(StringBuilder.ToString());

					StringBuilder.Length = 0;
				}
			}

			for (var i = 0; i != value.Length; ++i)
			{
				var c = value[i];

				if (c == (Char)StringCode.NEWLINE)
				{
					StringBuilder.Append(c);
					AddToken();
				}
				else if (c == (Char)StringCode.ENTER)
				{
					StringBuilder.Append(c);
					AddToken();
				}
				else if (c == (Char)StringCode.CLEAR)
				{
					StringBuilder.Append(c);
					AddToken();
				}
				else if (c == (Char)StringCode.COLOR)
				{
					AddToken();

					StringBuilder.Append(c);
					StringBuilder.Append(value[++i]);

					AddToken();
				}
				else if (c == (Char)StringCode.ITEM)
				{
					AddToken();

					StringBuilder.Append(c);
					StringBuilder.Append(value[++i]);

					AddToken();
				}
				else if (c == '#')
				{
					AddToken();

					StringBuilder.Append(c);

					var pump = LinqUtil.Pump(() => value[++i], @char => Char.IsLetter(@char) == false);
					foreach (var @char in pump) StringBuilder.Append(@char);

					StringBuilder.Append(value[i]);

					if (StringBuilder[StringBuilder.Length - 1] == 'R')
					{
						var pump2 = LinqUtil.Pump(() => value[++i], @char => @char != '#');
						foreach (var @char in pump2) StringBuilder.Append(@char);

						StringBuilder.Append(value[i]);
					}

					AddToken();
				}
				else
				{
					StringBuilder.Append(c);
				}
			}

			AddToken();

			return tokens;
		}

		public override Boolean CanRead => true;

		public override Boolean CanWrite => true;

		StringBuilder StringBuilder { get; }

		static Action<JsonWriter, JsonToken> InternalWriteValue { get; }
	}
}