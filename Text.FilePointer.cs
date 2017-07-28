using System;

namespace CrossbellTranslationTool.Text
{
	class FilePointer : IEquatable<FilePointer>, IComparable<FilePointer>
	{
		public FilePointer(FilePointerSize size, UInt32 position, UInt32 value)
		{
			Assert.IsValidEnumeration(size, nameof(size), true);

			Size = size;
			Position = position;
			Value = value;
		}

		public FilePointer Offset(UInt32 offsetposition, Int32 offsetamount)
		{
			var newposition = (UInt32)(Position > offsetposition ? Position + offsetamount : Position);
			var newvalue = (UInt32)(Value > offsetposition ? Value + offsetamount : Value);

			return (Position != newposition || Value != newvalue) ? new FilePointer(Size, newposition, newvalue) : this;
		}

		public override String ToString()
		{
			return $"{Position} - {Value}";
		}

		public override Boolean Equals(Object obj)
		{
			return (obj is FilePointer) ? this == (FilePointer)obj : false;
		}

		public override Int32 GetHashCode()
		{
			var hash = HashCode.InitialHashValue;
			hash = HashCode.CombineHashes(hash, Size.GetHashCode());
			hash = HashCode.CombineHashes(hash, Position.GetHashCode());
			hash = HashCode.CombineHashes(hash, Value.GetHashCode());

			return hash;
		}

		public Boolean Equals(FilePointer other)
		{
			return this == other;
		}

		public Int32 CompareTo(FilePointer other)
		{
			if (Object.ReferenceEquals(other, null) == true) return -1;

			return Position.CompareTo(other.Position);
		}

		public static Boolean operator ==(FilePointer lhs, FilePointer rhs)
		{
			if (lhs.Size != rhs.Size) return false;
			if (lhs.Position != rhs.Position) return false;
			if (lhs.Value != rhs.Value) return false;

			return true;
		}

		public static Boolean operator !=(FilePointer lhs, FilePointer rhs)
		{
			return !(lhs == rhs);
		}

		public FilePointerSize Size { get; }

		public UInt32 Position { get; }

		public UInt32 Value { get; }
	}
}