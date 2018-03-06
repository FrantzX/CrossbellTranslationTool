using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Text
{
	static class TextFileIO
	{
		public static List<String> Read(FileReader reader, FilePointerDelegate filepointerfunc, Int32 recordcount)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));
			Assert.Int32NotNegative(recordcount, nameof(recordcount));

			var output = new List<String>();

			var filepointers = new List<FilePointer>();
			var stringpointers = new List<FilePointer>();

			filepointerfunc(reader, recordcount, filepointers, stringpointers);

			foreach (var pointer in stringpointers)
			{
				if (pointer.Value > reader.Length) continue;

				reader.Position = pointer.Value;
				var str = reader.ReadString();
				output.Add(str);
			}

			return output;
		}

		public static Byte[] Write(FileReader reader, FilePointerDelegate filepointerfunc, Int32 recordcount, List<String> lines)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));
			Assert.Int32NotNegative(recordcount, nameof(recordcount));
			Assert.IsNotNull(lines, nameof(lines));

			var output = new List<Byte>();

			reader.Position = 0;
			output.AddRange(reader.ReadBytes((Int32)reader.Length));
			reader.Position = 0;

			var filepointers = new List<FilePointer>();
			var stringpointers = new List<FilePointer>();
			filepointerfunc(reader, recordcount, filepointers, stringpointers);

			var foo = filepointers.GroupBy(x => x.Position).Where(x => x.Count() > 1).ToList();

			var filepointermap = filepointers.ToDictionary(x => x.Position, x => x);
			var filepointermapkeys = filepointermap.Keys.ToList();

			var lineindex = 0;
			foreach (var stringpointer in stringpointers)
			{
				if (stringpointer.Value > reader.Length) continue;

				reader.Position = stringpointer.Value;
				var oldstr = reader.ReadString();
				var newstr = lines[lineindex++];

				if (newstr == "") continue;

				var oldstrbytes = reader.Encoding.GetBytes(oldstr);
				var newstrbytes = reader.Encoding.GetBytes(newstr);

				var sizedifference = newstrbytes.Length - oldstrbytes.Length;

				var saved = filepointermap[stringpointer.Position];

				foreach (var key in filepointermapkeys) filepointermap[key] = filepointermap[key].Offset(saved.Value, sizedifference);

				output.RemoveRange((Int32)saved.Value, oldstrbytes.Length);
				output.InsertRange((Int32)saved.Value, newstrbytes);
			}

			foreach (var pointer in filepointermap.Values)
			{
				if (pointer.Size == FilePointerSize.Size16)
				{
					output[(Int32)(pointer.Position + 0)] = (Byte)((pointer.Value & 0x00FF) >> 00);
					output[(Int32)(pointer.Position + 1)] = (Byte)((pointer.Value & 0xFF00) >> 08);
				}

				if (pointer.Size == FilePointerSize.Size32)
				{
					output[(Int32)(pointer.Position + 0)] = (Byte)((pointer.Value & 0x000000FF) >> 00);
					output[(Int32)(pointer.Position + 1)] = (Byte)((pointer.Value & 0x0000FF00) >> 08);
					output[(Int32)(pointer.Position + 2)] = (Byte)((pointer.Value & 0x00FF0000) >> 16);
					output[(Int32)(pointer.Position + 3)] = (Byte)((pointer.Value & 0xFF000000) >> 24);
				}
			}

			return output.ToArray();
		}
	}
}