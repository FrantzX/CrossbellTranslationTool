using System;
using System.Collections;

namespace CrossbellTranslationTool
{
	static class HashCode
	{
		public static Int32 CombineHashes(Int32 lhs, Int32 rhs)
		{
			return lhs * HashMultiplier ^ rhs;
		}

		public static Int32 Hash(params Object[] fields)
		{
			Assert.IsNotNull(fields, nameof(fields));

			unchecked
			{
				var hash = InitialHashValue;

				foreach (var item in fields)
				{
					if (item == null)
					{
						hash = CombineHashes(hash, 0);
					}
					else if (item is IEnumerable)
					{
						foreach (var subitem in (IEnumerable)item)
						{
							hash = CombineHashes(hash, subitem.GetHashCode());
						}
					}
					else
					{
						hash = CombineHashes(hash, item.GetHashCode());
					}
				}

				return hash;
			}
		}

		static public Int32 InitialHashValue { get; } = 17;

		static public Int32 HashMultiplier { get; } = 37;
	}
}