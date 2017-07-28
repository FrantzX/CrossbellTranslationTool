using System;

namespace CrossbellTranslationTool.Iso9660
{
	/// <summary>
	/// Represents the flags used in a <see cref="DirectoryRecord"/>.
	/// </summary>
	[Flags]
	enum DirectoryRecordFlags : byte
	{
		/// <summary>
		/// No value.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that the file is hidden from the user.
		/// </summary>
		Existence = 1,

		/// <summary>
		/// Indicates that the <see cref="DirectoryRecord"/> identifies a directory.
		/// </summary>
		Directory = 2,

		/// <summary>
		/// Indicates that the file is an associated file.
		/// </summary>
		AssociatedFile = 4,

		/// <summary>
		/// Indicates that the structure of the information in the file has a record format specified by a number other than zero  in the Record Format Field.
		/// </summary>
		Record = 8,

		/// <summary>
		/// Indicates that an owner identification and group identification are specifed for the file.
		/// </summary>
		Protection = 16,

		/// <summary>
		/// Indicates that this <see cref="DirectoryRecord"/> is not the final <see cref="DirectoryRecord"/> for the file.
		/// </summary>
		MultiExtent = 128
	}
}