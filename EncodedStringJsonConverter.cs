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

		public static List<String> SplitString(String input)
		{
			Assert.IsNotNull(input, nameof(input));

			var list = new List<String>();

			for (var i = 0; i < input.Length; ++i)
			{
				if (input[i] == '#')
				{
					for (var ii = i + 1; ii < input.Length; ++ii)
					{
						if (Char.IsDigit(input[ii]) == true)
						{
						}
						else if (Char.IsLetter(input[ii]) == true)
						{
							var str = input.Substring(i, ii - i + 1);
							list.Add(str);

							i = ii;
							break;
						}
						else
						{
							throw new Exception();
						}
					}
				}
				else if (input[i] < 0x20)
				{
					if (input[i] == (Byte)StringCode.COLOR)
					{
						var str = input.Substring(i, 2);
						list.Add(str);

						++i;
					}
					else if (input[i] == (Byte)StringCode.ITEM)
					{
						var str = input.Substring(i, 2);
						list.Add(str);

						++i;
					}
					else
					{
						var str = input.Substring(i, 1);
						list.Add(str);
					}
				}
				else
				{
					for (var ii = i + 1; ii < input.Length; ++ii)
					{
						if (input[ii] < 0x20)
						{
							var str = input.Substring(i, ii - i);
							list.Add(str);

							i = ii - 1;
							break;
						}
					}
				}
			}

			return list;
		}

		public override Boolean CanRead => false;

		public override Boolean CanWrite => true;

		static Action<JsonWriter, JsonToken> InternalWriteValue { get; }
	}
}