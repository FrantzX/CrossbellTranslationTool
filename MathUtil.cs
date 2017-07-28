using System;

namespace CrossbellTranslationTool
{
	/// <summary>
	/// A collection of math functions.
	/// </summary>
	static class MathUtil
	{
		/// <summary>
		/// Rounds a number up to a nearest multiple of another number.
		/// </summary>
		/// <param name="number">The number to be rounded up.</param>
		/// <param name="multiple">A number that is used as the basis of rounding.</param>
		/// <returns>A number that is <paramref name="number"/>, rounded up to the nearest multiple of <paramref name="multiple"/>.</returns>
		public static Int32 RoundUp(Int32 number, Int32 multiple)
		{
			return (number + (multiple - 1)) / multiple * multiple;
		}

		/// <summary>
		/// Rounds a number up to a nearest multiple of another number.
		/// </summary>
		/// <param name="number">The number to be rounded up.</param>
		/// <param name="multiple">A number that is used as the basis of rounding.</param>
		/// <returns>A number that is <paramref name="number"/>, rounded up to the nearest multiple of <paramref name="multiple"/>.</returns>
		public static UInt32 RoundUp(UInt32 number, UInt32 multiple)
		{
			return (number + (multiple - 1)) / multiple * multiple;
		}
	}
}