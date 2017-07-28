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
		}

		public static List<TextFileDescription> GetTextFileData()
		{
			var list = new List<TextFileDescription>();

			list.Add(new TextFileDescription("t_ittxt._dt", FilePointerReading.ReadFilePointers_ittxt));
			list.Add(new TextFileDescription("t_ittxt2._dt", FilePointerReading.ReadFilePointers_ittxt2));
			list.Add(new TextFileDescription("t_magic._dt", FilePointerReading.ReadFilePointers_magic));
			list.Add(new TextFileDescription("t_town._dt", FilePointerReading.ReadFilePointers_town));
			list.Add(new TextFileDescription("t_record._dt", FilePointerReading.ReadFilePointers_record));
			list.Add(new TextFileDescription("t_quest._dt", FilePointerReading.ReadFilePointers_quest));
			list.Add(new TextFileDescription("t_name._dt", FilePointerReading.ReadFilePointers_name));
			list.Add(new TextFileDescription("t_memo._dt", FilePointerReading.ReadFilePointers_memo));
			list.Add(new TextFileDescription("t_fish._dt", FilePointerReading.ReadFilePointers_fish));
			list.Add(new TextFileDescription("t_exvis._dt", FilePointerReading.ReadFilePointers_exvis));
			list.Add(new TextFileDescription("t_exmov._dt", FilePointerReading.ReadFilePointers_exmov));
			list.Add(new TextFileDescription("t_exchr._dt", FilePointerReading.ReadFilePointers_exchr));
			list.Add(new TextFileDescription("t_cook._dt", FilePointerReading.ReadFilePointers_cook));
			list.Add(new TextFileDescription("t_book00._dt", FilePointerReading.ReadFilePointers_book00));
			list.Add(new TextFileDescription("t_book01._dt", FilePointerReading.ReadFilePointers_book01));
			list.Add(new TextFileDescription("t_book02._dt", FilePointerReading.ReadFilePointers_book02));
			list.Add(new TextFileDescription("t_book03._dt", FilePointerReading.ReadFilePointers_book03));
			list.Add(new TextFileDescription("t_book04._dt", FilePointerReading.ReadFilePointers_book04));
			list.Add(new TextFileDescription("t_book05._dt", FilePointerReading.ReadFilePointers_book05));
			list.Add(new TextFileDescription("t_book06._dt", FilePointerReading.ReadFilePointers_book06));
			list.Add(new TextFileDescription("t_book07._dt", FilePointerReading.ReadFilePointers_book07));
			list.Add(new TextFileDescription("t_book08._dt", FilePointerReading.ReadFilePointers_book08));
			list.Add(new TextFileDescription("t_book09._dt", FilePointerReading.ReadFilePointers_book09));

			return list;
		}

		public String FileName { get; }

		public FilePointerDelegate FilePointerDelegate { get; }
	}
}