using System;

namespace CrossbellTranslationTool.Iso9660
{
	abstract class VolumeDescriptor : SectorObject
	{
		protected VolumeDescriptor()
		{
			VolumeDescriptorType = VolumeDescriptorType.Primary;
			StandardIdentifier = "CD001";
			VolumeDescriptorVersion = 1;
		}

		public VolumeDescriptorType VolumeDescriptorType { get; set; }

		public String StandardIdentifier { get; set; }

		public Byte VolumeDescriptorVersion { get; set; }
	}
}