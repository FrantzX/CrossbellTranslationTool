using System;
using System.Collections.Generic;
using System.Text;

namespace CrossbellTranslationTool.IO
{
	interface IFileSystem
	{
		List<String> GetChildren(String directorypath, String searchpattern);

		Boolean DoesFileExist(String filepath);

		FileReader OpenFile(String filepath, Encoding encoding);

		void SaveFile(String filepath, Byte[] buffer);
	}
}