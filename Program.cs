using System;
using CommandLine;

namespace CrossbellTranslationTool
{
	static class Program
	{
		static void Main(String[] args)
		{
			var parser = new Parser(settings => { settings.CaseSensitive = false; settings.HelpWriter = Console.Out; });

			var result = parser.ParseArguments<CommandLine.BuildArgs, CommandLine.ExtractArgs>(args);

			result.WithParsed<CommandLine.BuildArgs>(x => BuildAction.Run(x));
			result.WithParsed<CommandLine.ExtractArgs>(x => ExtractionAction.Run(x));
			result.WithNotParsed(x => { });
		}
	}
}