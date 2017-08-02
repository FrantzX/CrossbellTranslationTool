using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	[Verb("build", HelpText = "Inject translated text into the game.")]
	class BuildArgs : CommandLineArgs
	{
	}
}