using System;
using CommandLine;

namespace CrossbellTranslationTool.CommandLine
{
	[Verb("formatjson", HelpText = "Formats text in JSON files.")]
	class FormatJsonArgs
	{
		[Option('t', "translation", Required = true, HelpText = "Path to the translation files.")]
		public String TranslationPath { get; set; }
	}
}