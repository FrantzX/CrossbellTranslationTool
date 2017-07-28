using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	[Verb("extract", HelpText = "Extract strings from text files")]
	class ExtractArgs
	{
		[Value(0, Required = true, HelpText = "The identifier for the game.")]
		public String Game { get; set; } = "";

		[Value(1, Required = true, HelpText = "The path to the Ao no Kiseki PSP ISO.")]
		public String SourceIsoPath { get; set; } = "";

		[Value(2, Required = true, HelpText = "The path to the directory where to place the extracted text.")]
		public String DataPath { get; set; } = "";
	}
}