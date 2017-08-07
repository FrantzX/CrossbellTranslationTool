using System;
using System.IO;

namespace CrossbellTranslationTool.Actions
{
	static class FormatJson
	{
		public static void Run(CommandLine.FormatJsonArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			Console.WriteLine("Formatting JSON Files");
			Console.WriteLine($"Translation Location: {args.TranslationPath}");
			Console.WriteLine();

			foreach (var filepath in Directory.EnumerateFiles(args.TranslationPath, "*.json", SearchOption.AllDirectories))
			{
				var textitems = JsonTextItemFileIO.ReadFromFile(filepath);
				JsonTextItemFileIO.WriteToFile(textitems, filepath);
			}
		}
	}
}