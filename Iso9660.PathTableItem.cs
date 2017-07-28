using System;

namespace CrossbellTranslationTool.Iso9660
{
	class PathTableItem
	{
		public PathTableItem(String name, UInt32 sector, UInt16 parentindex)
		{
			Assert.IsValidString(name, nameof(name));

			Name = name;
			Sector = sector;
			ParentIndex = parentindex;
		}

		public String Name { get; set; }

		public UInt32 Sector { get; set; }

		public UInt16 ParentIndex { get; set; }
	}
}