using System;

namespace CrossbellTranslationTool
{
	static class IsoFilePaths
	{
		public static String DirectoryPath_Map4 { get; } = @"PSP_GAME\USRDIR\data\cclm\map4";

		public static String DirectoryPath_Text { get; } = @"PSP_GAME\USRDIR\data\text";

		public static String DirectoryPath_Scenario { get; } = @"PSP_GAME\USRDIR\data\scena";

		public static String DirectoryPath_BattleData { get; } = @"PSP_GAME\USRDIR\data\battle\dat";

		public static String FilePath_datalst { get; } = @"PSP_GAME\USRDIR\data.lst";

		public static String FilePath_sysstartbbc { get; } = @"PSP_GAME\USRDIR\data\cclm\system\sysstart.bcc";

		public static String FilePath_btasm1bbc { get; } = @"PSP_GAME\USRDIR\data\cclm\battle1\btasm1.bcc";

		public static String FilePath_sysonmembbc { get; } = @"PSP_GAME\USRDIR\data\cclm\system\sysonmem.bcc";

		public static String FilePath_monsnotedt2 { get; } = @"PSP_GAME\USRDIR\data\monsnote\monsnote.dt2";
	}
}