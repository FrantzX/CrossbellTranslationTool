using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	[Verb("extract", HelpText = "Extract text from the game.")]
	class ExtractArgs : CommandLineArgs
	{
	}
}