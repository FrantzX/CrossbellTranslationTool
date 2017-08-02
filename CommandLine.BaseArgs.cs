using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	abstract class CommandLineArgs
	{
		[Option('f', "format", Required = true, HelpText = "The game format. Either 'PSP' or 'PC'.")]
		public GameFormat Format { get; set; }

		[Option('g', "game", Required = true, HelpText = "The game. Either 'Ao' or 'Zero'. Only 'Ao' is supported.")]
		public Game Game { get; set; }

		[Option('t', "translation", Required = true, HelpText = "Path to the translation files.")]
		public String TranslationPath { get; set; }

		[Option('p', "pcdir", Default = "", HelpText = "Path to the PC game directory.", SetName = "pc")]
		public String GamePath { get; set; }

		[Option('s', "sourceiso", Default = "", HelpText = "Path to the source PSP ISO image file.", SetName = "iso")]
		public String SourceIsoPath { get; set; }

		[Option('d', "destinationiso", Default = "", HelpText = "Path where the destination PSP ISO image file will be placed.", SetName = "iso")]
		public String DestinationIsoPath { get; set; }
	}
}