using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool
{
	static class LinqUtil
	{
		public static T[] GetUnderlyingArray<T>(this List<T> list)
		{
			Assert.IsNotNull(list, nameof(list));

			var fieldinfo = list.GetType().GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return (T[])fieldinfo.GetValue(list);
		}

		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> function)
		{
			Assert.IsNotNull(sequence, nameof(sequence));
			Assert.IsNotNull(function, nameof(function));

			foreach (T obj in sequence) function(obj);
		}

		public static IList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence)
		{
			Assert.IsNotNull(sequence, nameof(sequence));

			if (sequence is System.Collections.ObjectModel.ReadOnlyCollection<T>)
			{
				return (System.Collections.ObjectModel.ReadOnlyCollection<T>)sequence;
			}

			if (sequence is IList<T>)
			{
				var list = (IList<T>)sequence;
				return new System.Collections.ObjectModel.ReadOnlyCollection<T>(list);
			}

			return sequence.ToList().AsReadOnly();
		}

		public static IEnumerable<T> Interspace<T>(this IEnumerable<T> sequence, T @object)
		{
			Assert.IsNotNull(sequence, nameof(sequence));

			return Interspace_Impl(sequence, @object);
		}

		static IEnumerable<T> Interspace_Impl<T>(this IEnumerable<T> sequence, T @object)
		{
			Boolean first = true;

			foreach (var item in sequence)
			{
				if (first == false) yield return @object;

				yield return item;
				first = false;
			}
		}

		public static IEnumerable<T> Pump<T>(Func<T> pump, Func<T, Boolean> condition)
		{
			Assert.IsNotNull(pump, nameof(pump));
			Assert.IsNotNull(condition, nameof(condition));

			return Pump_Impl(pump, condition);
		}

		public static IEnumerable<T> Pump<T>(Func<T> pump, Int32 count)
		{
			Assert.IsNotNull(pump, nameof(pump));

			return Pump_Impl(pump, count);
		}

		static IEnumerable<T> Pump_Impl<T>(Func<T> pump, Int32 count)
		{
			for (var index = 0; index != count; ++index)
			{
				yield return pump();
			}
		}

		static IEnumerable<T> Pump_Impl<T>(Func<T> pump, Func<T, Boolean> condition)
		{
			for (T obj = pump(); condition(obj) == true; obj = pump()) yield return obj;
		}

		public static IEnumerable<IEnumerable<T>> TakeAtATime<T>(this IEnumerable<T> sequence, Int32 count)
		{
			Assert.IsNotNull(sequence, nameof(sequence));
			Assert.IsTrue(count > 0, "Count must be greater than zero.");

			return TakeAtATime_Impl(sequence, count);
		}

		static IEnumerable<IEnumerable<T>> TakeAtATime_Impl<T>(IEnumerable<T> sequence, Int32 count)
		{
			T[] bucket = new T[count];
			var index = 0;

			foreach (T obj in sequence)
			{
				bucket[index++] = obj;

				if (index == count)
				{
					yield return bucket;
					index = 0;
				}
			}

			if (index > 0) yield return bucket;
		}
	}
}