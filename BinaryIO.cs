using System;
using System.Text;

namespace CrossbellTranslationTool
{
	/// <summary>
	/// Helper class containing methods for both reading and writing values from byte buffers.
	/// </summary>
	static class BinaryIO
	{
		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>The given value, as it is a single byte.</returns>
		public static Byte ChangeEndian(Byte value)
		{
			return value;
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="UInt16"/> value with the opposite endianness as the given value.</returns>
		public static UInt16 ChangeEndian(UInt16 value)
		{
			return (UInt16)((value & 0xFF) << 8 | (value & 0xFF00) >> 8);
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="UInt32"/> value with the opposite endianness as the given value.</returns>
		public static UInt32 ChangeEndian(UInt32 value)
		{
			return (value & 0xFF) << 24 | (value & 0xFF00) << 8 | (value & 0xFF0000) >> 8 | (value & 0xFF000000) >> 24;
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="UInt64"/> value with the opposite endianness as the given value.</returns>
		public static UInt64 ChangeEndian(UInt64 value)
		{
			unchecked
			{
				return ChangeEndian((UInt32)(value & (UInt64)(-1))) << 32 | ChangeEndian((UInt32)(value >> 32));
			}
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>The given value, as it is a single byte.</returns>
		public static SByte ChangeEndian(SByte value)
		{
			return value;
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="Int16"/> value with the opposite endianness as the given value.</returns>
		public static Int16 ChangeEndian(Int16 value)
		{
			return (Int16)ChangeEndian((UInt16)value);
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="Int32"/> value with the opposite endianness as the given value.</returns>
		public static Int32 ChangeEndian(Int32 value)
		{
			return (Int32)ChangeEndian((UInt32)value);
		}

		/// <summary>
		///Converts a small endian value in a big endian value, and vice-versa.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>A converted <see cref="Int64"/> value with the opposite endianness as the given value.</returns>
		public static Int64 ChangeEndian(Int64 value)
		{
			return (Int64)ChangeEndian((UInt64)value);
		}

		public static Byte ReadByteFromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			return buffer[offset];
		}

		public static UInt16 ReadUInt16FromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				return (UInt16)((buffer[offset + 0] << 0) | (buffer[offset + 1] << 8));
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				return (UInt16)((buffer[offset + 0] << 8) | (buffer[offset + 1] << 0));
			}

			throw new Exception();
		}

		public static UInt32 ReadUInt32FromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				return (UInt32)((buffer[offset + 0] << 0) | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				return (UInt32)((buffer[offset + 0] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | (buffer[offset + 3] << 0));
			}

			throw new Exception();
		}

		public static SByte ReadSByteFromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			return (SByte)buffer[offset];
		}

		public static Int16 ReadInt16FromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				return (Int16)((buffer[offset + 0] << 0) | (buffer[offset + 1] << 8));
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				return (Int16)((buffer[offset + 0] << 8) | (buffer[offset + 1] << 0));
			}

			throw new Exception();
		}

		public static Int32 ReadInt32FromBuffer(Byte[] buffer, Int32 offset, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				return (Int32)((buffer[offset + 0] << 0) | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				return (Int32)((buffer[offset + 0] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | (buffer[offset + 3] << 0));
			}

			throw new Exception();
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, Byte value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			buffer[offset] = value;
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, SByte value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			buffer[offset] = (Byte)value;
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, UInt32 value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				buffer[offset + 0] = (Byte)(value >> 0 & 255);
				buffer[offset + 1] = (Byte)(value >> 8 & 255);
				buffer[offset + 2] = (Byte)(value >> 16 & 255);
				buffer[offset + 3] = (Byte)(value >> 24 & 255);
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				buffer[offset + 0] = (Byte)(value >> 24 & 255);
				buffer[offset + 1] = (Byte)(value >> 16 & 255);
				buffer[offset + 2] = (Byte)(value >> 8 & 255);
				buffer[offset + 3] = (Byte)(value >> 0 & 255);
			}
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, Int32 value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			WriteIntoBuffer(buffer, offset, (UInt32)value, endian);
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, UInt16 value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			if (endian == Endian.LittleEndian || (endian == Endian.Default && BitConverter.IsLittleEndian))
			{
				buffer[offset + 0] = (Byte)(value >> 0 & 255);
				buffer[offset + 1] = (Byte)(value >> 8 & 255);
			}

			if (endian == Endian.BigEndian || (endian == Endian.Default && !BitConverter.IsLittleEndian))
			{
				buffer[offset + 0] = (Byte)(value >> 8 & 255);
				buffer[offset + 1] = (Byte)(value >> 0 & 255);
			}
		}

		/// <summary>
		/// Writes a given value into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="value">The value to be written.</param>
		/// <param name="endian">The endianness used in writing the value. Defaults to <see cref="Endian.Default"/></param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, Int16 value, Endian endian = Endian.Default)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsValidEnumeration(endian, nameof(endian), false);

			WriteIntoBuffer(buffer, offset, (UInt16)value, endian);
		}

		/// <summary>
		/// Writes a given string value into a given buffer.
		/// </summary>
		/// <param name="buffer">The buffer where the value is to be written into.</param>
		/// <param name="offset">The offset in to the given buffer where the value is to be written into.</param>
		/// <param name="encoding">The encoding used to convert the given string into bytes.</param>
		/// <param name="value">The string to be written into the buffer.</param>
		/// <param name="length">The total number of bytes to be written into the buffer.</param>
		/// <param name="padding">Padding value to be used after writing the string if the length of the string in bytes is less than <paramref name="offset"/>.</param>
		public static void WriteIntoBuffer(Byte[] buffer, Int32 offset, Encoding encoding, String value, Int32 length, Char padding)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsNotNull(encoding, nameof(encoding));
			Assert.IsNotNull(value, nameof(value));

			var writecount = encoding.GetBytes(value, 0, Math.Min(length, value.Length), buffer, offset);
			if (writecount < length)
			{
				var paddinglength = length - writecount;
				for (var i = 0; i != paddinglength; ++i) buffer[offset + writecount + i] = (Byte)padding;
			}
		}
	}
}