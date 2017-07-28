using System;
using System.Diagnostics;

namespace CrossbellTranslationTool.Iso9660
{
	/// <summary>
	/// Represents the first sector of an ISO9660 that contains file data.
	/// </summary>
	class FileSector : SectorObject
	{
		public FileSector(DirectoryRecord record, Byte[] buffer)
		{
			Assert.IsNotNull(record, nameof(record));
			Assert.IsNotNull(buffer, nameof(buffer));

			Record = record;

			m_data = buffer;
		}

		public FileSector(DirectoryRecord record, Func<Byte[]> dataloader)
		{
			Assert.IsNotNull(record, nameof(record));
			Assert.IsNotNull(dataloader, nameof(dataloader));

			Record = record;

			m_data = null;
			m_dataloader = dataloader;
		}

		public FileSector(DirectoryRecord record)
		{
			Assert.IsNotNull(record, nameof(record));

			Record = record;

			m_data = new Byte[0];
			m_dataloader = null;
		}

		public Byte[] GetData()
		{
			if(m_data == null)
			{
				m_data = m_dataloader();
			}

			return m_data;
		}

		public void SetData(Byte[] buffer)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			m_data = buffer;
		}

		/// <summary>
		/// The <see cref="DirectoryRecord"/> that identifies this file.
		/// </summary>
		public DirectoryRecord Record { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Byte[] m_data;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Func<Byte[]> m_dataloader;

		#endregion
	}
}