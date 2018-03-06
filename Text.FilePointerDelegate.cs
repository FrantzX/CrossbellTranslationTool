using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Text
{
	delegate void FilePointerDelegate(FileReader reader, Int32 recordcount, List<FilePointer> allpointers, List<FilePointer> stringpointers);
}