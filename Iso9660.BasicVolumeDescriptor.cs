using System;

namespace CrossbellTranslationTool.Iso9660
{
	class BasicVolumeDescriptor : VolumeDescriptor
	{
		public String SystemIdentifier { get; set; }

		public String VolumeIdentifier { get; set; }

		public UInt32 VolumeSpaceSize { get; set; }

		public UInt16 VolumeSetSize { get; set; }

		public UInt16 VolumeSequenceNumber { get; set; }

		public UInt16 LogicalBlockSize { get; set; }

		public UInt32 PathTableSize { get; set; }

		public UInt32 TypeLPathTableLocation { get; set; }

		public UInt32 OptionalTypeLPathTableLocation { get; set; }

		public UInt32 TypeMPathTableLocation { get; set; }

		public UInt32 OptionalTypeMPathTableLocation { get; set; }

		public DirectoryRecord RootDirectory { get; set; }

		public String VolumeSetIdentifier { get; set; }

		public String PublisherIdentifier { get; set; }

		public String DataPreparerIdentifier { get; set; }

		public String ApplicationIdentifier { get; set; }

		public String CopyrightFileIdentifier { get; set; }

		public String AbstractFileIdentifier { get; set; }

		public String BibliographicFileIdentifier { get; set; }

		public DateTimeOffset CreationDateAndTime { get; set; }

		public DateTimeOffset ModificationDateAndTime { get; set; }

		public DateTimeOffset ExpirationDateAndTime { get; set; }

		public DateTimeOffset EffectiveDateAndTime { get; set; }

		public Byte FileStructureVersion { get; set; }
	}
}