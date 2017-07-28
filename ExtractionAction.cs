using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrossbellTranslationTool
{
	static class ExtractionAction
	{
		public static void Run(CommandLine.ExtractArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			Console.WriteLine("Extracting text");
			Console.WriteLine($"Source ISO: {args.SourceIsoPath}");
			Console.WriteLine($"Output Location: {args.DataPath}");
			Console.WriteLine();

			var data_text = Text.TextFileDescription.GetTextFileData();
			var data_scenario = ScenarioFileList.GetList();

			using (var iso = new Iso9660.IsoImage(args.SourceIsoPath))
			{
				var totalscenastringtable = new SortedSet<String>();

				Directory.CreateDirectory(args.DataPath);
				Directory.CreateDirectory(Path.Combine(args.DataPath, "text"));
				Directory.CreateDirectory(Path.Combine(args.DataPath, "scena"));

				foreach (var item in data_text)
				{
					Console.WriteLine(item.FileName);

					var textfilepath = Path.Combine(IsoFilePaths.DirectoryPath_Text, item.FileName);
					var jsonfilepath = Path.ChangeExtension(Path.Combine(args.DataPath, "text", item.FileName), ".json");

					var strings = ReadTextFile(iso, textfilepath, item.FilePointerDelegate);
					JsonTextItemFileIO.WriteToFile(strings.Select(x => new TextItem(x)).ToList(), jsonfilepath);
				}

				foreach (var item in data_scenario)
				{
					Console.WriteLine(item);

					var scenariofilepath = Path.Combine(IsoFilePaths.DirectoryPath_Scenario, item);
					var jsonfilepath = Path.ChangeExtension(Path.Combine(args.DataPath, "scena", item), ".json");

					var strings = ReadScenarioFile(iso, scenariofilepath, jsonfilepath);

					JsonTextItemFileIO.WriteToFile(strings.Item1.Select(x => new TextItem(x)).ToList(), jsonfilepath);

					strings.Item2.Where(x => String.IsNullOrWhiteSpace(x) == false).ForEach(x => totalscenastringtable.Add(x));
				}

				JsonTextItemFileIO.WriteToFile(totalscenastringtable.Select(x => new TextItem(x)).ToList(), Path.Combine(args.DataPath, "stringtable.json"));
			}

			Console.WriteLine();
			Console.WriteLine("Done.");
		}

		static Tuple<List<String>, List<String>> ReadScenarioFile(Iso9660.IsoImage iso, String filepath, String jsonpath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			var file = iso.GetFile(filepath);
			var sourcebytes = file.GetData();
			var scenariofile = new ScenarioFile(new FileReader(sourcebytes));

			var stringlist = new List<String>();
			scenariofile.VisitOperands(x => x.Type == Bytecode.OperandType.String, x => stringlist.Add(x.GetValue<String>()));

			var stringtable = scenariofile.GetStringTable();

			return Tuple.Create(stringlist, stringtable);
		}

		static List<String> ReadTextFile(Iso9660.IsoImage iso, String filepath, Text.FilePointerDelegate filepointerfunc)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));

			var file = iso.GetFile(filepath);
			var bytes = file.GetData();

			using (var reader = new FileReader(bytes))
			{
				var strings = Text.TextFileIO.Read(reader, filepointerfunc);
				return strings;
			}
		}
	}
}