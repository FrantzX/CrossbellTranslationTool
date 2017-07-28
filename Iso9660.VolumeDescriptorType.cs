using System;

namespace CrossbellTranslationTool.Iso9660
{
	enum VolumeDescriptorType : byte
	{
		Boot = 0,
		Primary = 1,
		Supplementary = 2,
		Partition = 3,
		SetTerminator = 255
	}
}