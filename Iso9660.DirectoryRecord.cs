using System;

namespace CrossbellTranslationTool.Iso9660
{
	class DirectoryRecord
	{
		public DirectoryRecord()
		{
			FileIdentifier = "";
		}

		public Byte GetSize()
		{
			return (Byte)(MathUtil.RoundUp(33 + FileIdentifier.Length, 2) + (SystemUseData != null ? SystemUseData.Length : 0));
		}

		public override String ToString()
		{
			return FileIdentifier;
		}

		public Byte ExtendedAttributeRecordLength { get; set; }

		public UInt32 SectorNumber { get; set; }

		public UInt32 DataLength { get; set; }

		public DateTimeOffset RecordingDateAndTime { get; set; }

		public DirectoryRecordFlags Flags { get; set; }

		public Byte FileUnitSize { get; set; }

		public Byte InterleaveGapSize { get; set; }

		public UInt16 VolumeSequenceNumber { get; set; }

		public String FileIdentifier { get; set; }

		public Byte[] SystemUseData { get; set; }
	}
}
