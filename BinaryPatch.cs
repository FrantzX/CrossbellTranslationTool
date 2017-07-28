using System;

namespace CrossbellTranslationTool
{
	class BinaryPatch
	{
		public BinaryPatchType Type { get; set; }
		public UInt32 Offset { get; set; }

		public Int32 Count { get; set; }
		public String Text { get; set; }
		public Byte[] Buffer { get; set; }
	}
}