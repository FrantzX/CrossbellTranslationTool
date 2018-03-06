using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CrossbellTranslationTool.Actions
{
	static class Extract
	{
		public static void Run(CommandLine.ExtractArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			if (ValidateArgs(args) == false) return;

			if (args.Format == GameFormat.PSP)
			{
				Console.WriteLine("Extracting text");
				Console.WriteLine($"Source ISO: {args.SourceIsoPath}");
				Console.WriteLine($"Translation Location: {args.TranslationPath}");
				Console.WriteLine();

				using (var iso = new Iso9660.IsoImage(args.SourceIsoPath))
				{
					var filesystem = new IO.IsoFileSystem(iso, @"PSP_GAME\USRDIR");

					Run(args.Game, filesystem, Encodings.ShiftJIS, args.TranslationPath);
				}
			}

			if (args.Format == GameFormat.PC)
			{
				Console.WriteLine("Extracting text");
				Console.WriteLine($"Game Directory: {args.GamePath}");
				Console.WriteLine($"Translation Location: {args.TranslationPath}");
				Console.WriteLine();

				var filesystem = new IO.DirectoryFileSystem(args.GamePath);

				Run(args.Game, filesystem, Encodings.Chinese, args.TranslationPath);
			}
		}

		static Boolean ValidateArgs(CommandLine.ExtractArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			if (args.Game == Game.None) return false;
			if (args.Format == GameFormat.None) return false;
			if (args.Format == GameFormat.PSP && args.SourceIsoPath == "") return false;
			if (args.Format == GameFormat.PC && args.GamePath == "") return false;

			return true;
		}

		static void Run(Game game, IO.IFileSystem filesystem, Encoding encoding, String datapath)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(filesystem, nameof(filesystem));
			Assert.IsNotNull(encoding, nameof(encoding));
			Assert.IsValidString(datapath, nameof(datapath));

			var data_text = Text.TextFileDescription.GetTextFileData(game);

			var totalscenastringtable = new SortedSet<String>();

			Directory.CreateDirectory(datapath);
			Directory.CreateDirectory(Path.Combine(datapath, "text"));
			Directory.CreateDirectory(Path.Combine(datapath, "scena"));
			Directory.CreateDirectory(Path.Combine(datapath, "monster"));

			foreach (var item in data_text)
			{
				Console.WriteLine(item.FileName);

				var textfilepath = Path.Combine(@"data\text", item.FileName);
				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "text", item.FileName), ".json");

				using (var reader = filesystem.OpenFile(textfilepath, encoding))
				{
					var strings = Text.TextFileIO.Read(reader, item.FilePointerDelegate, item.RecordCount);
					JsonTextItemFileIO.WriteToFile(strings.Select(x => new TextItem(x)).ToList(), jsonfilepath);
				}
			}

			foreach (var filepath in filesystem.GetChildren(@"data\scena", "*.bin"))
			{
				var filename = Path.GetFileName(filepath);

				Console.WriteLine(filename);

				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "scena", filename), ".json");

				using (var reader = filesystem.OpenFile(filepath, encoding))
				{
					var strings = ReadScenarioFile(game, reader);

					JsonTextItemFileIO.WriteToFile(strings.Item1.Select(x => new TextItem(x)).ToList(), jsonfilepath);

					strings.Item2.Where(x => String.IsNullOrWhiteSpace(x) == false).ForEach(x => totalscenastringtable.Add(x));
				}
			}

			JsonTextItemFileIO.WriteToFile(totalscenastringtable.Select(x => new TextItem(x)).ToList(), Path.Combine(datapath, "stringtable.json"));

			foreach (var filepath in filesystem.GetChildren(@"data\battle\dat", "ms*.dat"))
			{
				var filename = Path.GetFileName(filepath);
				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "monster", filename), ".json");

				Console.WriteLine(filename);

				using (var reader = filesystem.OpenFile(filepath, encoding))
				{
					var monsterfile = OpenMonsterDefinitionFile(game, reader);
					var strings = monsterfile.GetStrings();

					JsonTextItemFileIO.WriteToFile(strings.Select(x => new TextItem(x)).ToList(), jsonfilepath);
				}
			}

			Console.WriteLine();
			Console.WriteLine("Done.");
		}

		static Tuple<List<String>, List<String>> ReadScenarioFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			var scenariofile = OpenScenarioFile(game, reader);

			var stringlist = new List<String>();
			scenariofile.VisitOperands(x => x.Type == Bytecode.OperandType.String, x => stringlist.Add(x.GetValue<String>()));

			var stringtable = scenariofile.GetStringTable();

			return Tuple.Create(stringlist, stringtable);
		}

		static IMonsterDefinitionFile OpenMonsterDefinitionFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			switch (game)
			{
				case Game.Ao:
					return new MonsterDefinitionFile_Ao(reader);

				case Game.Zero:
					return new MonsterDefinitionFile_Zero(reader);

				default:
					throw new Exception();
			}

		}

		static ScenarioFile OpenScenarioFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			switch (game)
			{
				case Game.Ao:
					return new ScenarioFile(reader, typeof(Bytecode.InstructionTable_AoKScena));

				case Game.Zero:
					return new ScenarioFile(reader, typeof(Bytecode.InstructionTalble_ZoKScena));

				default:
					throw new Exception();
			}
		}
	}
}