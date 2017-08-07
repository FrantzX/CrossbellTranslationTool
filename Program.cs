using System;
using CommandLine;

namespace CrossbellTranslationTool
{
	static class Program
	{
		static void Main(String[] args)
		{
			var parser = new Parser(settings => { settings.CaseSensitive = false; settings.HelpWriter = Console.Out; });

			var result = parser.ParseArguments<CommandLine.BuildArgs, CommandLine.ExtractArgs, CommandLine.FormatJsonArgs>(args);

			result.WithParsed<CommandLine.BuildArgs>(x => Actions.Build.Run(x));
			result.WithParsed<CommandLine.ExtractArgs>(x => Actions.Extract.Run(x));
			result.WithParsed<CommandLine.FormatJsonArgs>(x => Actions.FormatJson.Run(x));
			result.WithNotParsed(x => { });
		}
	}
}