using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Iso9660
{
	/// <summary>
	/// Represents a ISO9660 image file sector that contains <see cref="DirectoryRecord"/>.
	/// </summary>
	class DirectoryRecordSector : SectorObject
	{
		public DirectoryRecordSector()
		{
			Records = new List<DirectoryRecord>();
		}

		public List<DirectoryRecord> Records { get; }
	}
}