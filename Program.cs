using System;
using CommandLine;
using System.Linq;

namespace CrossbellTranslationTool
{
	static class Program
	{
		static void Main(String[] args)
		{
			//ZeroCheck();
			//return;

			var parser = new Parser(settings => { settings.CaseSensitive = false; settings.HelpWriter = Console.Out; });

			var result = parser.ParseArguments<CommandLine.BuildArgs, CommandLine.ExtractArgs, CommandLine.FormatJsonArgs>(args);

			result.WithParsed<CommandLine.BuildArgs>(x => Actions.Build.Run(x));
			result.WithParsed<CommandLine.ExtractArgs>(x => Actions.Extract.Run(x));
			result.WithParsed<CommandLine.FormatJsonArgs>(x => Actions.FormatJson.Run(x));
			result.WithNotParsed(x => { });
		}

		static void ZeroCheck()
		{
			
			var args = new CommandLine.ExtractArgs();
			//args.Game = Game.Zero;
			//args.Format = GameFormat.PSP;
			//args.SourceIsoPath = @"D:\ISOs\psp - Zero no Kiseki (JPN).iso";
			//args.TranslationPath = @"D:\Code\Crossbell Translations\Zero no Kiseki ~ PSP";

			args.Game = Game.Zero;
			args.Format = GameFormat.PC;
			args.GamePath = @"D:\ED_ZERO";
			args.TranslationPath = @"D:\Code\Crossbell Translations\Zero no Kiseki ~ PC";

			Actions.Extract.Run(args);

	


			/*
			var args = new CommandLine.BuildArgs();
			args.Game = Game.Ao;
			args.Format = GameFormat.PC;
			args.GamePath = @"D:\ED_AO\";
			args.TranslationPath = @"D:\Code\Ao no Kiseki Translation\PC";
			Actions.Build.Run(args);
			*/

		/*
			var jpn_filepath = @"D:\ISOs\Zero no Kiseki\PSP_GAME\USRDIR\data\battle\dat\ms00000.dat";
			var eng_filepath = @"D:\ISOs\Zero no Kiseki (ENG)\PSP_GAME\USRDIR\data\battle\dat\ms00000.dat";

			var jpn_scena = new MonsterDefinitionFile_Zero(new FileReader(jpn_filepath, Encodings.ShiftJIS));
			var eng_scena = new MonsterDefinitionFile_Zero(new FileReader(eng_filepath, Encodings.ShiftJIS));
	*/

		}

		static void Do()
		{
			var scenapdir = @"D:\ED_AO\data\scena";
			var jsondir = @"D:\Code\Ao no Kiseki Translation\PC\scena";
			var jsondir2 = @"D:\Code\Ao no Kiseki Translation\PC\scena2";

			foreach (var scenafilepath in System.IO.Directory.EnumerateFiles(scenapdir, "*.bin"))
			{
				var scenafilename = System.IO.Path.GetFileName(scenafilepath);
				var jsonfilename = System.IO.Path.ChangeExtension(scenafilename, ".json");
				var jsonfilepath = System.IO.Path.Combine(jsondir, jsonfilename);
				var jsonfilepath2 = System.IO.Path.Combine(jsondir2, jsonfilename);

				var scena = new ScenarioFile(new FileReader(scenafilepath, Encodings.Chinese), typeof(Bytecode.InstructionTable_AoKScena));

				var textitems = JsonTextItemFileIO.ReadFromFile(jsonfilepath);
				var scenastrings = scena.GetFunctionStrings();

				var scenatextitems = scenastrings.Select(x => x.Select(y => new TextItem(y)).ToList()).ToList();

				foreach (var item in Enumerable.Zip(textitems, scenatextitems.SelectMany(x => x), (lhs, rhs) => new { lhs, rhs }))
				{
					if (item.lhs.Translation != "") item.rhs.Translation = item.lhs.Translation;
				}

				var obj = new { Functions = scenatextitems.Select(x => new { Name = "", Strings = x }), StringTable = scena.GetStringTable() };

				var jsonsettings = new Newtonsoft.Json.JsonSerializerSettings();
				jsonsettings.Formatting = Newtonsoft.Json.Formatting.Indented;
				jsonsettings.Converters.Add(new EncodedStringJsonConverter());

				var jsontext = Newtonsoft.Json.JsonConvert.SerializeObject(obj, jsonsettings);

				System.IO.File.Delete(jsonfilepath2);
				System.IO.File.WriteAllText(jsonfilepath2, jsontext);
			}
		}
	}
}