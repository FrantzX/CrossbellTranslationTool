using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Iso9660
{
	class PathTable : SectorObject
	{
		public PathTable(Endian endian)
		{
			Assert.IsValidEnumeration(endian, nameof(endian), true);

			Items = new List<PathTableItem>();
			Endian = endian;
		}

		public List<PathTableItem> Items { get; }

		public Endian Endian { get; set; }
	}
}