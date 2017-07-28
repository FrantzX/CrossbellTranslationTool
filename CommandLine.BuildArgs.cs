using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	[Verb("build", HelpText = "Build a new ISO file")]
	class BuildArgs
	{
		[Value(0, Required = true, HelpText = "The identifier for the game.")]
		public String Game { get; set; } = "";

		[Value(1, Required = true, HelpText = "The path to the source ISO image file.")]
		public String SourceIsoPath { get; set; } = "";

		[Value(2, Required = true, HelpText = "The path where a new ISO image file will be created.")]
		public String DestinationIsoPath { get; set; } = "";

		[Value(3, Required = true, HelpText = "The path to the directory where to get the extracted text.")]
		public String DataPath { get; set; } = "";
	}
}