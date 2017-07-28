using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace CrossbellTranslationTool
{
	/// <summary>
	/// Assertion class with methods used for parameter validation.
	/// </summary>
	static class Assert
	{
		/// <summary>
		/// Validation method that throws an exception if given object is null.
		/// </summary>
		/// <typeparam name="T">The type of the given object.</typeparam>
		/// <param name="value">The object to be checked.</param>
		/// <param name="name">The name of the object.</param>
		[DebuggerStepThrough]
		public static void IsNotNull<T>(T value, String name)
		{
			if (Object.ReferenceEquals(value, null) == true) throw new ArgumentNullException(name ?? "Unknown Object");
		}

		/// <summary>
		/// Validation method that throws and exception if given string is null or empty.
		/// </summary>
		/// <param name="value">The string to be checked.</param>
		/// <param name="name">The name of the string.</param>
		[DebuggerStepThrough]
		public static void IsValidString(String value, String name)
		{
			if (String.IsNullOrEmpty(value) == true) throw new ArgumentException("String cannot be null or empty.", name ?? "Unknown String");
		}

		/// <summary>
		/// Validation method that throws and exception if the given enumerated value is not definied.
		/// </summary>
		/// <typeparam name="T">The type of the given value.</typeparam>
		/// <param name="value">The value to be validated.</param>
		/// <param name="name">The name of the value.</param>
		/// <param name="notzero">If true, the given value also must not be zero.</param>
		[DebuggerStepThrough]
		public static void IsValidEnumeration<T>(T value, String name, Boolean notzero) where T : struct
		{
			if (typeof(T).IsEnum == false) throw new ArgumentException("T is not an enumeration", nameof(value));

			var type = typeof(T);

			if (IsFlagsEnumCache.ContainsKey(type) == false)
			{
				IsFlagsEnumCache.Add(type, Attribute.IsDefined(type, typeof(FlagsAttribute)));
			}

			if (IsFlagsEnumCache[type] == true)
			{
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
				Int32 totalflags = fields.Aggregate(0, (seed, field) => (Int32)field.GetValue(null) | seed);
				Int32 intvalue = (Int32)(Object)value;

				if ((totalflags & intvalue) != intvalue) throw new ArgumentException("Value is not valid enumeration", name ?? "Unknown Enumeration");
			}
			else
			{
				if (Enum.IsDefined(type, value) == false) throw new ArgumentException("Value is not valid enumeration", name ?? "Unknown Enumeration");
			}

			if (notzero == true && Object.Equals(value, Enum.ToObject(type, 0)) == true) throw new ArgumentException("Enumerated value cannot be zero", name ?? "Unknown Enumeration");
		}

		/// <summary>
		/// Validation methos that throws an exception is the given boolean value is false.
		/// </summary>
		/// <param name="value">The boolean value to be validated.</param>
		/// <param name="errortext">The text to be used in the exception.</param>
		[DebuggerStepThrough]
		public static void IsTrue(Boolean value, String errortext)
		{
			if (value == false) throw new Exception(errortext);
		}

		/// <summary>
		/// Dictionary used to cache whether a type is a <see cref="Enum"/> with a <see cref="FlagsAttribute"/>
		/// </summary>
		static Dictionary<Type, Boolean> IsFlagsEnumCache { get; } = new Dictionary<Type, Boolean>();
	}
}