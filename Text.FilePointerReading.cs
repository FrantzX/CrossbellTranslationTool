using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Text
{
	static class FilePointerReading
	{
		public static void ReadFilePointers_book(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != recordcount; ++i)
			{
				var ptr = reader.ReadFilePointer16();

				allpointers.Add(ptr);
				stringpointers.Add(ptr);
			}
		}

		public static void ReadFilePointers_cook_AoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Position = 4;

			for (var i = 0; i != 25; ++i)
			{
				var ptr = reader.ReadFilePointer16();

				allpointers.Add(ptr);
				stringpointers.Add(ptr);

				reader.Position += 58;
			}
		}

		public static void ReadFilePointers_cook_ZoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			for (var i = 0; i != recordcount; ++i)
			{
				reader.Position = 0x30 * (i + 1);

				var ptr = reader.ReadFilePointer16();

				allpointers.Add(ptr);
				stringpointers.Add(ptr);
			}
		}

		public static void ReadFilePointers_exchr(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != recordcount; ++i)
			{
				reader.Stream.Position += 12;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				allpointers.Add(ptr1);
				allpointers.Add(ptr2);

				stringpointers.Add(ptr1);
				stringpointers.Add(ptr2);
			}

			reader.Stream.Position += 8;

			var ptr1x = reader.ReadFilePointer16();
			var ptr2x = reader.ReadFilePointer16();

			allpointers.Add(ptr1x);
			allpointers.Add(ptr2x);

			stringpointers.Add(ptr1x);
			stringpointers.Add(ptr2x);
		}

		public static void ReadFilePointers_exmov(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != recordcount; ++i)
			{
				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				allpointers.Add(ptr1);
				allpointers.Add(ptr2);

				stringpointers.Add(ptr1);
				stringpointers.Add(ptr2);

				reader.Stream.Position += 4;
			}
		}

		public static void ReadFilePointers_exvis_AoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != 53; ++i)
			{
				reader.Stream.Position += 12;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				allpointers.Add(ptr1);
				allpointers.Add(ptr2);

				stringpointers.Add(ptr1);
				stringpointers.Add(ptr2);
			}

			reader.Stream.Position += 8;

			var ptr1x = reader.ReadFilePointer16();
			var ptr2x = reader.ReadFilePointer16();

			allpointers.Add(ptr1x);
			allpointers.Add(ptr2x);

			stringpointers.Add(ptr1x);
			stringpointers.Add(ptr2x);
		}

		public static void ReadFilePointers_exvis_ZoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != 70; ++i)
			{
				reader.Position = (i * 12);

				reader.Position += 8;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				allpointers.Add(ptr1);
				allpointers.Add(ptr2);

				stringpointers.Add(ptr1);
				stringpointers.Add(ptr2);
			}

			reader.Position += 6;

			var ptr1_ = reader.ReadFilePointer16();
			var ptr2_ = reader.ReadFilePointer16();

			allpointers.Add(ptr1_);
			allpointers.Add(ptr2_);

			stringpointers.Add(ptr1_);
			stringpointers.Add(ptr2_);
		}

		public static void ReadFilePointers_fish_AoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Position = 0;

			void Foo1()
			{
				reader.Position = 6;

				var ptr = reader.ReadFilePointer16();
				allpointers.Add(ptr);

				reader.Position = ptr.Value;

				var list = LinqUtil.Pump(() => reader.ReadFilePointer16(), 5).ToList();

				allpointers.AddRange(list);
				stringpointers.AddRange(list);
			}

			void Foo2()
			{
				reader.Position = 8;

				var ptr = reader.ReadFilePointer16();
				allpointers.Add(ptr);

				reader.Position = ptr.Value;

				var list = LinqUtil.Pump(() => reader.ReadFilePointer16(), 5).ToList();

				allpointers.AddRange(list);
			}

			void Foo3()
			{
				reader.Position = 10;

				var ptr = reader.ReadFilePointer16();
				allpointers.Add(ptr);

				reader.Position = ptr.Value;

				var list = LinqUtil.Pump(() => reader.ReadFilePointer16(), 120).ToList();

				allpointers.AddRange(list);
				stringpointers.AddRange(list);
			}

			void Foo4()
			{
				reader.Position = 12;

				for (var i = 0; i != 31; ++i)
				{
					var num = reader.ReadUInt16();
					var stringpointer = reader.ReadFilePointer16();

					allpointers.Add(stringpointer);
					stringpointers.Add(stringpointer);

					reader.Position += 56;
				}
			}

			Foo1();
			Foo2();
			Foo3();
			Foo4();
		}

		public static void ReadFilePointers_fish_ZoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			for (var i = 0; i != recordcount; ++i)
			{
				reader.Position = 0x06 + (i * 60);

				var id = reader.ReadUInt16();

				var strptr = reader.ReadFilePointer16();

				allpointers.Add(strptr);
				stringpointers.Add(strptr);

			}
		}

		public static void ReadFilePointers_memo(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			for (var i = 0; i != recordcount; ++i)
			{
				var stringpointer = reader.ReadFilePointer16();

				allpointers.Add(stringpointer);
				stringpointers.Add(stringpointer);
			}
		}

		public static void ReadFilePointers_name(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			while (true)
			{
				var index = reader.ReadUInt16();
				var stringpointer = reader.ReadFilePointer16();

				allpointers.Add(stringpointer);
				stringpointers.Add(stringpointer);

				reader.Stream.Position += 16;

				if (index == 999) break;
			}

		}

		public static void ReadFilePointers_quest(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			var questpointers = new List<Tuple<FilePointer, FilePointer, FilePointer, FilePointer>>();

			reader.Stream.Position = 0;

			while (true)
			{
				var key = reader.ReadByte();

				reader.Stream.Position += 11;

				var ptr1 = reader.ReadFilePointer32();
				var ptr2 = reader.ReadFilePointer32();
				var ptr3 = reader.ReadFilePointer32();
				var ptr4 = reader.ReadFilePointer32();

				questpointers.Add(Tuple.Create(ptr1, ptr2, ptr3, ptr4));

				if (key == 255) break;
			}

			for (var i = 0; i != questpointers.Count; ++i)
			{
				var item = questpointers[i];

				allpointers.Add(item.Item1);
				allpointers.Add(item.Item2);
				allpointers.Add(item.Item3);
				allpointers.Add(item.Item4);

				stringpointers.Add(item.Item1);
				stringpointers.Add(item.Item2);
				stringpointers.Add(item.Item3);

				var v4_start = item.Item4.Value;
				var v4_end = i != questpointers.Count - 1 ? questpointers[i + 1].Item4.Value : reader.Stream.Length;

				for (var offset = v4_start; offset != v4_end; offset += 4)
				{
					reader.Stream.Position = offset;

					var v4pointer = reader.ReadFilePointer32();

					allpointers.Add(v4pointer);
					stringpointers.Add(v4pointer);
				}
			}
		}

		public static void ReadFilePointers_ittxt(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0x14;

			var recordpositions = LinqUtil.Pump(() => reader.ReadFilePointer16(), x => x.Value != 0).ToList();

			foreach (var position_record in recordpositions)
			{
				allpointers.Add(position_record);

				reader.Stream.Position = position_record.Value + 4;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				if (allpointers.Exists(x => x.Position == ptr1.Position && x.Value == ptr1.Value) == false) allpointers.Add(ptr1);
				if (allpointers.Exists(x => x.Position == ptr2.Position && x.Value == ptr2.Value) == false) allpointers.Add(ptr2);

				if (stringpointers.Exists(x => x.Position == ptr1.Position && x.Value == ptr1.Value) == false) stringpointers.Add(ptr1);
				if (stringpointers.Exists(x => x.Position == ptr2.Position && x.Value == ptr2.Value) == false) stringpointers.Add(ptr2);
			}
		}

		public static void ReadFilePointers_ittxt2(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			var initoffset = reader.ReadUInt16();

			reader.Position = initoffset;

			var recordpositions = new List<FilePointer>();

			while (true)
			{
				var pointer = reader.ReadFilePointer16();

				if (recordpositions.Count > 0 && recordpositions.Last().Value > pointer.Value) break;

				recordpositions.Add(pointer);
			}

			foreach (var position_record in recordpositions)
			{
				allpointers.Add(position_record);

				reader.Stream.Position = position_record.Value + 4;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				if (allpointers.Contains(ptr1) == false) allpointers.Add(ptr1);
				if (allpointers.Contains(ptr2) == false) allpointers.Add(ptr2);

				if (stringpointers.Contains(ptr1) == false) stringpointers.Add(ptr1);
				if (stringpointers.Contains(ptr2) == false) stringpointers.Add(ptr2);
			}
		}

		public static void ReadFilePointers_magic_AoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0x0;

			var recordpositions = LinqUtil.Pump(() => reader.ReadFilePointer16(), x => x.Value != 0).ToList();

			foreach (var position_record in recordpositions)
			{
				allpointers.Add(position_record);

				reader.Stream.Position = position_record.Value;

				var key = reader.ReadUInt16();
				if (key == 0) continue;

				reader.Stream.Position = position_record.Value + 24;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				if (allpointers.Exists(x => x.Position == ptr1.Position && x.Value == ptr1.Value) == false) allpointers.Add(ptr1);
				if (allpointers.Exists(x => x.Position == ptr2.Position && x.Value == ptr2.Value) == false) allpointers.Add(ptr2);

				if (stringpointers.Exists(x => x.Position == ptr1.Position && x.Value == ptr1.Value) == false) stringpointers.Add(ptr1);
				if (stringpointers.Exists(x => x.Position == ptr2.Position && x.Value == ptr2.Value) == false) stringpointers.Add(ptr2);
			}
		}

		public static void ReadFilePointers_magic_ZoK(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0x0;

			var recordpositions = LinqUtil.Pump(() => reader.ReadFilePointer16(), x => x.Value != 0).ToList();

			foreach (var position_record in recordpositions)
			{
				allpointers.Add(position_record);

				reader.Stream.Position = position_record.Value;

				var key = reader.ReadUInt16();
				if (key == 0) continue;

				reader.Stream.Position = position_record.Value + 28;

				var ptr1 = reader.ReadFilePointer16();
				var ptr2 = reader.ReadFilePointer16();

				if (ptr1.Value >= reader.Length) continue;
				if (ptr2.Value >= reader.Length) continue;

				if (allpointers.Contains(ptr1) == false) allpointers.Add(ptr1);
				if (allpointers.Contains(ptr2) == false) allpointers.Add(ptr2);

				if (stringpointers.Contains(ptr1) == false) stringpointers.Add(ptr1);
				if (stringpointers.Contains(ptr2) == false) stringpointers.Add(ptr2);
			}
		}

		public static void ReadFilePointers_town(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0;

			var count = reader.ReadInt16();

			for (var i = 0; i != count; ++i)
			{
				var stringpointer = reader.ReadFilePointer16();

				allpointers.Add(stringpointer);
				stringpointers.Add(stringpointer);
			}
		}

		public static void ReadFilePointers_record(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers)
		{
			reader.Stream.Position = 0x04;

			for (var i = 0; i != recordcount; ++i)
			{
				var id = reader.ReadUInt32();

				var textpointer1 = reader.ReadFilePointer16();
				var textpointer2 = reader.ReadFilePointer16();

				allpointers.Add(textpointer1);
				allpointers.Add(textpointer2);

				stringpointers.Add(textpointer1);
				stringpointers.Add(textpointer2);

				reader.Stream.Position += 4;
			}
		}
	}
}