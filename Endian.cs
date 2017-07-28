using System;

namespace CrossbellTranslationTool
{
	/// <summary>
	/// Describes the sequential order used to numerically interpret a range of bytes in computer memory as a larger, composed word value.
	/// </summary>
	enum Endian
	{
		/// <summary>
		/// The default endian value of the running computer.
		/// </summary>
		Default,

		/// <summary>
		/// Ordered from the least significant byte to the most significant byte. 
		/// </summary>
		LittleEndian,

		/// <summary>
		/// Ordered from the most significant byte to the least significant byte.
		/// </summary>
		BigEndian
	}
}