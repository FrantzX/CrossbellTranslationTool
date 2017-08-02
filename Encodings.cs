using System;
using System.Text;

namespace CrossbellTranslationTool
{
	/// <summary>
	/// Helper class containing text encodings used.
	/// </summary>
	static class Encodings
	{
		/// <summary>
		/// Gets an encoding for the ASCII (7-bit) character set.
		/// </summary>
		public static Encoding ASCII { get; } = Encoding.ASCII;

		/// <summary>
		/// Gets an encoding for the multibyte Shift-JIS character set.
		/// </summary>
		public static Encoding ShiftJIS { get; } = Encoding.GetEncoding("Shift-JIS");

		/// <summary>
		/// Gets an encoding for the multibyte Chinese Simplified character set.
		/// </summary>
		public static Encoding Chinese { get; } = Encoding.GetEncoding(936);
	}
}