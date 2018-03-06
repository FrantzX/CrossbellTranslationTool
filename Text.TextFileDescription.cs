using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Text
{
	class TextFileDescription
	{
		public TextFileDescription(String filename, FilePointerDelegate @delegate)
		{
			Assert.IsValidString(filename, nameof(filename));
			Assert.IsNotNull(@delegate, nameof(@delegate));

			FileName = filename;
			FilePointerDelegate = @delegate;
			RecordCount = 0;
		}

		public TextFileDescription(String filename, FilePointerDelegate @delegate, Int32 recordcount)
		{
			Assert.IsValidString(filename, nameof(filename));
			Assert.IsNotNull(@delegate, nameof(@delegate));
			Assert.Int32NotNegative(recordcount, nameof(recordcount));

			FileName = filename;
			FilePointerDelegate = @delegate;
			RecordCount = recordcount;
		}

		public static List<TextFileDescription> GetTextFileData(Game game)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);

			switch (game)
			{
				case Game.Ao:
					return GetTextFileData_AoK();

				case Game.Zero:
					return GetTextFileData_ZoK();

				default:
					throw new Exception();
			}
		}

		public static List<TextFileDescription> GetTextFileData_ZoK()
		{
			var list = new List<TextFileDescription>();

			list.Add(new TextFileDescription("t_ittxt._dt", FilePointerReading.ReadFilePointers_ittxt));
			list.Add(new TextFileDescription("t_ittxt2._dt", FilePointerReading.ReadFilePointers_ittxt2));
			list.Add(new TextFileDescription("t_magic._dt", FilePointerReading.ReadFilePointers_magic_ZoK));
			list.Add(new TextFileDescription("t_town._dt", FilePointerReading.ReadFilePointers_town));
			list.Add(new TextFileDescription("t_record._dt", FilePointerReading.ReadFilePointers_record, 49));
			list.Add(new TextFileDescription("t_quest._dt", FilePointerReading.ReadFilePointers_quest));
			list.Add(new TextFileDescription("t_name._dt", FilePointerReading.ReadFilePointers_name));
			list.Add(new TextFileDescription("t_memo._dt", FilePointerReading.ReadFilePointers_memo, 83));
			list.Add(new TextFileDescription("t_fish._dt", FilePointerReading.ReadFilePointers_fish_ZoK, 24));
			list.Add(new TextFileDescription("t_exvis._dt", FilePointerReading.ReadFilePointers_exvis_ZoK));
			list.Add(new TextFileDescription("t_exmov._dt", FilePointerReading.ReadFilePointers_exmov, 9));
			list.Add(new TextFileDescription("t_exchr._dt", FilePointerReading.ReadFilePointers_exchr, 59));
			list.Add(new TextFileDescription("t_cook._dt", FilePointerReading.ReadFilePointers_cook_ZoK, 24));
			list.Add(new TextFileDescription("t_book00._dt", FilePointerReading.ReadFilePointers_book, 6));
			list.Add(new TextFileDescription("t_book01._dt", FilePointerReading.ReadFilePointers_book, 18));
			list.Add(new TextFileDescription("t_book02._dt", FilePointerReading.ReadFilePointers_book, 14));
			list.Add(new TextFileDescription("t_book03._dt", FilePointerReading.ReadFilePointers_book, 14));
			list.Add(new TextFileDescription("t_book04._dt", FilePointerReading.ReadFilePointers_book, 18));
			list.Add(new TextFileDescription("t_book05._dt", FilePointerReading.ReadFilePointers_book, 8));

			return list;
		}

		public static List<TextFileDescription> GetTextFileData_AoK()
		{
			var list = new List<TextFileDescription>();

			list.Add(new TextFileDescription("t_ittxt._dt", FilePointerReading.ReadFilePointers_ittxt));
			list.Add(new TextFileDescription("t_ittxt2._dt", FilePointerReading.ReadFilePointers_ittxt2));
			list.Add(new TextFileDescription("t_magic._dt", FilePointerReading.ReadFilePointers_magic_AoK));
			list.Add(new TextFileDescription("t_town._dt", FilePointerReading.ReadFilePointers_town));
			list.Add(new TextFileDescription("t_record._dt", FilePointerReading.ReadFilePointers_record, 57));
			list.Add(new TextFileDescription("t_quest._dt", FilePointerReading.ReadFilePointers_quest));
			list.Add(new TextFileDescription("t_name._dt", FilePointerReading.ReadFilePointers_name));
			list.Add(new TextFileDescription("t_memo._dt", FilePointerReading.ReadFilePointers_memo, 120));
			list.Add(new TextFileDescription("t_fish._dt", FilePointerReading.ReadFilePointers_fish_AoK));
			list.Add(new TextFileDescription("t_exvis._dt", FilePointerReading.ReadFilePointers_exvis_AoK));
			list.Add(new TextFileDescription("t_exmov._dt", FilePointerReading.ReadFilePointers_exmov, 15));
			list.Add(new TextFileDescription("t_exchr._dt", FilePointerReading.ReadFilePointers_exchr, 118));
			list.Add(new TextFileDescription("t_cook._dt", FilePointerReading.ReadFilePointers_cook_AoK));
			list.Add(new TextFileDescription("t_book00._dt", FilePointerReading.ReadFilePointers_book, 6));
			list.Add(new TextFileDescription("t_book01._dt", FilePointerReading.ReadFilePointers_book, 18));
			list.Add(new TextFileDescription("t_book02._dt", FilePointerReading.ReadFilePointers_book, 14));
			list.Add(new TextFileDescription("t_book03._dt", FilePointerReading.ReadFilePointers_book, 14));
			list.Add(new TextFileDescription("t_book04._dt", FilePointerReading.ReadFilePointers_book, 18));
			list.Add(new TextFileDescription("t_book05._dt", FilePointerReading.ReadFilePointers_book, 8));
			list.Add(new TextFileDescription("t_book06._dt", FilePointerReading.ReadFilePointers_book, 18));
			list.Add(new TextFileDescription("t_book07._dt", FilePointerReading.ReadFilePointers_book, 8));
			list.Add(new TextFileDescription("t_book08._dt", FilePointerReading.ReadFilePointers_book, 14));
			list.Add(new TextFileDescription("t_book09._dt", FilePointerReading.ReadFilePointers_book, 14));

			return list;
		}

		public String FileName { get; }

		public FilePointerDelegate FilePointerDelegate { get; }

		public Int32 RecordCount { get; }
	}
}