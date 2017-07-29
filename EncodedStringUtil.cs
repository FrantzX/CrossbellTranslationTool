using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossbellTranslationTool
{
	static class EncodedStringUtil
	{
		public static Int32 GetSize(String input)
		{
			Assert.IsNotNull(input, nameof(input));

			var count = 0;

			for (var i = 0; i != input.Length; ++i)
			{
				var @char = input[i];
				if (@char > 0x20)
				{
					count += (@char > 0x7F) ? 2 : 1;
				}
				else if (@char == (Byte)StringCode.COLOR)
				{
					++i;
					count += 2;
				}
				else if (@char == (Byte)StringCode.ITEM)
				{
					++i;
					count += 3;
				}
				else
				{
					++count;
				}
			}

			return count + 1;
		}

		public static String ReadString(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			var output = "";
			var bytelist = new List<Byte>();

			while (true)
			{
				var b = reader.ReadByte();

				if (b == 0)
				{
					break;
				}
				else if (b < 0x20)
				{
					output += Encodings.ShiftJIS.GetString(bytelist.ToArray());
					bytelist.Clear();

					if (b == (Byte)StringCode.COLOR)
					{
						var value = reader.ReadByte();

						output += (Char)b;
						output += (Char)value;
					}
					else if (b == (Byte)StringCode.ITEM)
					{
						var value = reader.ReadUInt16();

						output += (Char)b;
						output += (Char)value;
					}
					else
					{
						output += (Char)b;
					}
				}
				else
				{
					bytelist.Add(b);
				}
			}

			output += Encodings.ShiftJIS.GetString(bytelist.ToArray());
			bytelist.Clear();

			return output;
		}

		public static String GetStringForJSON(String input)
		{
			Assert.IsNotNull(input, nameof(input));

			var builder = new System.Text.StringBuilder();

			builder.Append('\"');

			for (var i = 0; i != input.Length; ++i)
			{
				var @char = input[i];
				if (@char == '\\')
				{
					builder.Append(@"\\");
				}
				else if (@char == '\"')
				{
					builder.Append("\\\"");
				}
				else if (@char >= 0x20)
				{
					builder.Append(@char);
				}
				else if (@char == (Byte)StringCode.COLOR)
				{
					++i;
					var data = input[i];

					builder.Append($"\\u{(UInt16)@char:X4}");
					builder.Append($"\\u{(UInt16)data:X4}");
				}
				else if (@char == (Byte)StringCode.ITEM)
				{
					++i;
					var data = input[i];

					builder.Append($"\\u{(UInt16)@char:X4}");
					builder.Append($"\\u{(UInt16)data:X4}");
				}
				else
				{
					builder.Append($"\\u{(UInt16)@char:X4}");
				}
			}

			builder.Append('\"');

			return builder.ToString();
		}

		public static Byte[] GetBytes(String input)
		{
			Assert.IsNotNull(input, nameof(input));

			var buffer = new Byte[GetSize(input)];

			var offset = 0;
			for (var i = 0; i != input.Length; ++i)
			{
				var @char = input[i];
				if (@char > 0x20)
				{
					offset += Encodings.ShiftJIS.GetBytes(input, i, 1, buffer, offset);
				}
				else if (@char == (Byte)StringCode.COLOR)
				{
					buffer[offset + 0] = (Byte)@char;
					buffer[offset + 1] = (Byte)input[i + 1];

					i += 1;
					offset += 2;
				}
				else if (@char == (Byte)StringCode.ITEM)
				{
					buffer[offset + 0] = (Byte)@char;
					BinaryIO.WriteIntoBuffer(buffer, offset + 1, (UInt16)input[i + 1], Endian.LittleEndian);

					i += 1;
					offset += 3;
				}
				else
				{
					buffer[offset] = (Byte)@char;
					offset += 1;
				}
			}

			return buffer;
		}

		public static void WriteStringToStream(Stream stream, String value)
		{
			Assert.IsNotNull(stream, nameof(stream));
			Assert.IsNotNull(value, nameof(value));

			var bytes = GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
