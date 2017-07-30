using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Iso9660
{
	/// <summary>
	/// Class which represents an ISO9660 image. This does not implement the entire spec, but enough to read and write PSP ISO images.
	/// </summary>
	class IsoImage : IDisposable
	{
		/// <summary>
		/// Opens the given file as an ISO9660 image.
		/// </summary>
		/// <param name="filepath">Filepath to the ISO9660 image file.</param>
		public IsoImage(String filepath)
		{
			Assert.IsNotNull(filepath, nameof(filepath));

			SectorMap = new SortedDictionary<UInt32, SectorObject>();
			Reader = new FileReader(filepath);

			Reader.Position = DefaultSectorSize * 16;

			var volumedescriptors = ReadVolumeDescriptors();

			var primaryvolumedescriptor = GetPrimaryVolumeDescriptor();

			var pathtableLE = ReadPathTable(primaryvolumedescriptor, Endian.LittleEndian);
			var pathtableBE = ReadPathTable(primaryvolumedescriptor, Endian.BigEndian);

			SectorMap[primaryvolumedescriptor.TypeLPathTableLocation] = pathtableLE;
			SectorMap[primaryvolumedescriptor.OptionalTypeLPathTableLocation] = pathtableLE;

			SectorMap[primaryvolumedescriptor.TypeMPathTableLocation] = pathtableBE;
			SectorMap[primaryvolumedescriptor.OptionalTypeMPathTableLocation] = pathtableBE;

			DirectoryRecordTree = ReadDirectoryRecords(primaryvolumedescriptor);

		}

		/// <summary>
		/// Closes the ISO9660 image file.
		/// </summary>
		public void Dispose()
		{
			Reader.Dispose();
		}

		/// <summary>
		/// Saves the ISO9660 image to a file.
		/// </summary>
		/// <param name="filepath">The filepath where to save the image.</param>
		public void Save(String filepath)
		{
			Assert.IsValidString(filepath, nameof(filepath));

			using (var filestream = File.Open(filepath, FileMode.Create))
			using (var writer = new BinaryWriter(filestream))
			{
				foreach (var item in SectorMap)
				{
					writer.BaseStream.Position = item.Key * DefaultSectorSize;

					switch (item.Value)
					{
						case BasicVolumeDescriptor obj:
							WriteBasicVolumeDescriptor(writer, obj);
							break;

						case SetTerminatorVolumeDescriptor obj:
							WriteSetTerminatorVolumeDescriptor(writer, obj);
							break;

						case PathTable obj:
							WritePathTable(writer, obj);
							break;

						case DirectoryRecordSector obj:
							WriteDirectoryRecordSector(writer, obj);
							break;

						case FileSector obj:
							WriteFileSector(writer, obj);
							break;

						default:
							throw new Exception();
					}
				}

				var primaryvolumedescriptor = GetPrimaryVolumeDescriptor();

				filestream.SetLength(primaryvolumedescriptor.LogicalBlockSize * primaryvolumedescriptor.VolumeSpaceSize);
			}
		}

		/// <summary>
		/// Return the primary volume descriptor of this ISO9660 image, which should be the first object in the image.
		/// </summary>
		/// <returns>The primary volume descriptor of this ISO9660 image.</returns>
		public BasicVolumeDescriptor GetPrimaryVolumeDescriptor()
		{
			return (BasicVolumeDescriptor)SectorMap.First().Value;
		}

		/// <summary>
		/// Returns the <see cref="FileSector"/> object for a given file.
		/// </summary>
		/// <param name="filepath">The path to the file requested.</param>
		/// <returns>If the file exists, then the <see cref="FileSector"/> object that represents the file. Otherwise, null.</returns>
		public FileSector GetFile(String filepath)
		{
			Assert.IsNotNull(filepath, nameof(filepath));

			var record = GetRecord(filepath);
			if (record == null) return null;

			return (FileSector)SectorMap[record.SectorNumber];
		}

		/// <summary>
		/// Returns the <see cref="DirectoryRecord"/> object for a given file.
		/// </summary>
		/// <param name="filepath">The path to the file requested.</param>
		/// <returns>If the file exists, then the <see cref="DirectoryRecord"/> of that file. Otherwise, null.</returns>
		public DirectoryRecord GetRecord(String filepath)
		{
			Assert.IsNotNull(filepath, nameof(filepath));

			var root = DirectoryRecordTree.Keys.First(x => x.FileIdentifier == "\0");
			var splitpath = filepath.Split('\\');

			var dir = root;
			foreach (var item in splitpath)
			{
				var found = DirectoryRecordTree[dir].FirstOrDefault(x => x.FileIdentifier == item);
				if (found == null) return null;

				dir = found;
			}

			return dir;
		}

		/// <summary>
		/// Reuturn the children <see cref="DirectoryRecord"/>s of a directory.
		/// </summary>
		/// <param name="directorypath">The path to directory whose children are requested.</param>
		/// <returns>A <see cref="List{DirectoryRecord}"/> containing the child <see cref="DirectoryRecord"/>s of the given directory.</returns>
		public List<DirectoryRecord> GetChildren(String directorypath)
		{
			Assert.IsValidString(directorypath, nameof(directorypath));

			var directory = GetRecord(directorypath);
			if (directory == null) return null;

			return DirectoryRecordTree[directory];
		}

		/// <summary>
		/// Moves a file in the image to a different sector. Updates the sector map and the <see cref="DirectoryRecord"/> of the file. This method does not perform any validation on whether the file can be moved to the given sector.
		/// </summary>
		/// <param name="filesector">The <see cref="FileSector"/> to be moved.</param>
		/// <param name="newsector">The sector to move the <see cref="FileSector"/> to.</param>
		public void ChangeFileSector(FileSector filesector, UInt32 newsector)
		{
			Assert.IsNotNull(filesector, nameof(filesector));

			SectorMap.Remove(filesector.Record.SectorNumber);

			SectorMap[newsector] = filesector;

			filesector.Record.SectorNumber = newsector;
		}

		/// <summary>
		/// Returns the highest sector in use by the ISO9660 image. This method assumes that this sector will be in use by a <see cref="FileSector"/>.
		/// </summary>
		/// <returns>The highest sector used by the image.</returns>
		public UInt32 GetHighestSectorUsed()
		{
			var lastsectoritem = SectorMap.Last();

			if (lastsectoritem.Value is FileSector file)
			{
				var lastsectorused = lastsectoritem.Key + (MathUtil.RoundUp(file.Record.DataLength, (UInt32)DefaultSectorSize) / DefaultSectorSize);
				return (UInt32)lastsectorused;
			}

			throw new Exception();
		}

		/// <summary>
		/// Checks if there is available room in the image for a <see cref="DirectoryRecord"/> at its current sector.
		/// </summary>
		/// <param name="record">The <see cref="DirectoryRecord"/> to be checked.</param>
		/// <param name="sectorsneedded">The number of sectors needed by the <see cref="DirectoryRecord"/>.</param>
		/// <returns>Return true if there is available room in the image for the file at its current sector. Otherwise, false.</returns>
		public Boolean CheckForRoom(DirectoryRecord record, Int32 sectorsneedded)
		{
			Assert.IsNotNull(record, nameof(record));

			var sectorsneedsed = Enumerable.Range((Int32)record.SectorNumber, sectorsneedded).Skip(1);

			foreach (var sector in sectorsneedsed)
			{
				if (SectorMap.ContainsKey((UInt32)sector) == true) return false;
			}

			return true;
		}

		#region ISO Reading

		/// <summary>
		/// Reads the <see cref="VolumeDescriptor"/>s found in the image file at the current position until a <see cref="SetTerminatorVolumeDescriptor"/> is found. Also inserts the <see cref="VolumeDescriptor"/>s found into the sector map.
		/// </summary>
		/// <returns>A list containing the <see cref="VolumeDescriptor"/>s found in the image file.</returns>
		List<VolumeDescriptor> ReadVolumeDescriptors()
		{
			var list = new List<VolumeDescriptor>();

			while (true)
			{
				var sector = Reader.Position / DefaultSectorSize;

				var descriptor = ReadVolumeDescriptor();

				list.Add(descriptor);
				SectorMap[(UInt32)sector] = descriptor;

				if (descriptor is SetTerminatorVolumeDescriptor) break;
			}

			return list;
		}

		/// <summary>
		/// Reads the <see cref="VolumeDescriptor"/> at the current position of the image file.
		/// </summary>
		/// <returns>The <see cref="VolumeDescriptor"/> located at the current file position.</returns>
		VolumeDescriptor ReadVolumeDescriptor()
		{
			var buffer = Reader.ReadBytes(DefaultSectorSize);
			var type = (VolumeDescriptorType)buffer[0];

			if (type == VolumeDescriptorType.Primary)
			{
				var descriptor = new BasicVolumeDescriptor();
				descriptor.VolumeDescriptorType = type;
				descriptor.StandardIdentifier = Encodings.ASCII.GetString(buffer, 1, 5);
				descriptor.VolumeDescriptorVersion = buffer[6];

				descriptor.SystemIdentifier = Encodings.ASCII.GetString(buffer, 8, 32).Trim(' ');
				descriptor.VolumeIdentifier = Encodings.ASCII.GetString(buffer, 40, 32).Trim(' ');
				descriptor.VolumeSpaceSize = BinaryIO.ReadUInt32FromBuffer(buffer, 80, Endian.LittleEndian);
				descriptor.VolumeSetSize = BinaryIO.ReadUInt16FromBuffer(buffer, 120, Endian.LittleEndian);
				descriptor.VolumeSequenceNumber = BinaryIO.ReadUInt16FromBuffer(buffer, 124, Endian.LittleEndian);
				descriptor.LogicalBlockSize = BinaryIO.ReadUInt16FromBuffer(buffer, 128, Endian.LittleEndian);
				descriptor.PathTableSize = BinaryIO.ReadUInt32FromBuffer(buffer, 132, Endian.LittleEndian);
				descriptor.TypeLPathTableLocation = BinaryIO.ReadUInt32FromBuffer(buffer, 140, Endian.LittleEndian);
				descriptor.OptionalTypeLPathTableLocation = BinaryIO.ReadUInt32FromBuffer(buffer, 144, Endian.LittleEndian);
				descriptor.TypeMPathTableLocation = BinaryIO.ReadUInt32FromBuffer(buffer, 148, Endian.BigEndian);
				descriptor.OptionalTypeMPathTableLocation = BinaryIO.ReadUInt32FromBuffer(buffer, 152, Endian.BigEndian);
				descriptor.RootDirectory = ReadDirectoryRecord(buffer, 156);
				descriptor.VolumeSetIdentifier = Encodings.ASCII.GetString(buffer, 190, 128).Trim(' ');
				descriptor.PublisherIdentifier = Encodings.ASCII.GetString(buffer, 318, 128).Trim(' ');
				descriptor.DataPreparerIdentifier = Encodings.ASCII.GetString(buffer, 446, 128).Trim(' ');
				descriptor.ApplicationIdentifier = Encodings.ASCII.GetString(buffer, 574, 128).Trim(' ');
				descriptor.CopyrightFileIdentifier = Encodings.ASCII.GetString(buffer, 702, 37).Trim(' ');
				descriptor.AbstractFileIdentifier = Encodings.ASCII.GetString(buffer, 739, 37).Trim(' ');
				descriptor.BibliographicFileIdentifier = Encodings.ASCII.GetString(buffer, 776, 37).Trim(' ');
				descriptor.CreationDateAndTime = ReadVolumeDescriptorDateTime(buffer, 813);
				descriptor.ModificationDateAndTime = ReadVolumeDescriptorDateTime(buffer, 830);
				descriptor.ExpirationDateAndTime = ReadVolumeDescriptorDateTime(buffer, 847);
				descriptor.EffectiveDateAndTime = ReadVolumeDescriptorDateTime(buffer, 864);
				descriptor.FileStructureVersion = buffer[881];

				return descriptor;
			}

			if (type == VolumeDescriptorType.SetTerminator)
			{
				var descriptor = new SetTerminatorVolumeDescriptor();
				descriptor.VolumeDescriptorType = type;
				descriptor.StandardIdentifier = Encodings.ASCII.GetString(buffer, 1, 5);
				descriptor.VolumeDescriptorVersion = buffer[6];

				return descriptor;
			}

			throw new Exception();
		}

		PathTable ReadPathTable(BasicVolumeDescriptor descriptor, Endian endian)
		{
			Assert.IsNotNull(descriptor, nameof(descriptor));
			Assert.IsTrue(endian == Endian.BigEndian || endian == Endian.LittleEndian, "Endian must be set to either little or big.");

			if (endian == Endian.BigEndian)
			{
				Reader.Position = descriptor.TypeMPathTableLocation * DefaultSectorSize;
			}
			else
			{
				Reader.Position = descriptor.TypeLPathTableLocation * DefaultSectorSize;
			}

			var buffer = Reader.ReadBytes((Int32)descriptor.PathTableSize);

			var pathtable = new PathTable(endian);

			for (var offset = 0; offset < buffer.Length;)
			{
				var namelength = buffer[offset + 0];
				var extendedsectors = buffer[offset + 1];
				var sector = BinaryIO.ReadUInt32FromBuffer(buffer, offset + 2, endian);
				var parentindex = BinaryIO.ReadUInt16FromBuffer(buffer, offset + 6, endian);
				var name = Encodings.ASCII.GetString(buffer, offset + 8, namelength).Trim(' ');

				pathtable.Items.Add(new PathTableItem(name, sector, parentindex));

				offset += MathUtil.RoundUp(8 + namelength, 2);
			}

			return pathtable;
		}

		Dictionary<DirectoryRecord, List<DirectoryRecord>> ReadDirectoryRecords(BasicVolumeDescriptor descriptor)
		{
			Assert.IsNotNull(descriptor, nameof(descriptor));

			var directorytree = new Dictionary<DirectoryRecord, List<DirectoryRecord>>();

			var workinglist = new Stack<DirectoryRecord>();
			workinglist.Push(descriptor.RootDirectory);

			while (workinglist.Count > 0)
			{
				var dir = workinglist.Pop();

				var children = ReadChildrenDirectoryRecords(dir);

				var sectorobj = new DirectoryRecordSector();
				sectorobj.Records.AddRange(children);

				SectorMap[dir.SectorNumber] = sectorobj;
				directorytree[dir] = new List<DirectoryRecord>();

				foreach (var child in children.Skip(2))
				{
					if ((child.Flags & DirectoryRecordFlags.Directory) == DirectoryRecordFlags.Directory)
					{
						workinglist.Push(child);
					}
					else
					{
						SectorMap[child.SectorNumber] = new FileSector(child, () => FileSectorReadFunc((Int32)child.SectorNumber, (Int32)child.DataLength));
					}

					directorytree[dir].Add(child);
				}
			}

			return directorytree;
		}

		List<DirectoryRecord> ReadChildrenDirectoryRecords(DirectoryRecord currentdirectory)
		{
			Assert.IsNotNull(currentdirectory, nameof(currentdirectory));

			Reader.Position = currentdirectory.SectorNumber * DefaultSectorSize;

			var records = new List<DirectoryRecord>();
			var buffer = new Byte[DefaultSectorSize];

			for (var offset = 0; offset < currentdirectory.DataLength; offset += DefaultSectorSize)
			{
				var bytesread = Reader.Read(buffer, 0, DefaultSectorSize);

				var readoffset = 0;

				while (readoffset < buffer.Length && buffer[readoffset] != 0)
				{
					var data = ReadDirectoryRecord(buffer, readoffset);

					records.Add(data);

					readoffset += data.GetSize();
				}
			}

			return records;
		}

		/// <summary>
		/// Reads a <see cref="DirectoryRecord" from a buffer./>
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">The offset in the buffer to read from.</param>
		/// <returns></returns>
		DirectoryRecord ReadDirectoryRecord(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var length = (Int32)buffer[offset];
			var record = new DirectoryRecord();

			record.ExtendedAttributeRecordLength = buffer[offset + 1];
			record.SectorNumber = BinaryIO.ReadUInt32FromBuffer(buffer, offset + 2);
			record.DataLength = BinaryIO.ReadUInt32FromBuffer(buffer, offset + 10);
			record.RecordingDateAndTime = ReadDirectoryRecordDateTime(buffer, offset + 18);
			record.Flags = (DirectoryRecordFlags)buffer[offset + 25];
			record.FileUnitSize = buffer[offset + 26];
			record.InterleaveGapSize = buffer[offset + 27];
			record.VolumeSequenceNumber = BinaryIO.ReadUInt16FromBuffer(buffer, offset + 28);

			var textlength = buffer[offset + 32];
			record.FileIdentifier = Encodings.ASCII.GetString(buffer, offset + 33, textlength).Trim(' ');

			var num2 = ((textlength & 1) == 0) ? 1 : 0;
			var num3 = textlength + num2 + 33;
			var num4 = length - num3;

			if (num4 > 0)
			{
				record.SystemUseData = new Byte[num4];
				Array.Copy(buffer, offset + num3, record.SystemUseData, 0, num4);
			}

			return record;
		}

		/// <summary>
		/// Reads a <see cref="DateTimeOffset"/> from a buffer in <see cref="DirectoryRecord"/> format.
		/// </summary>
		/// <param name="buffer">The buffer from which to read a <see cref="DateTimeOffset"/>.</param>
		/// <param name="offset">The offset in the buffer to read the <see cref="DateTimeOffset"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/></returns>
		DateTimeOffset ReadDirectoryRecordDateTime(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			try
			{
				var time = new DateTimeOffset(1900 + buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3], buffer[offset + 4], buffer[offset + 5], TimeSpan.FromMinutes(15 * (SByte)buffer[offset + 6]));
				return time;
			}
			catch (ArgumentOutOfRangeException)
			{
				return DateTimeOffset.MinValue;
			}
		}

		/// <summary>
		/// Reads a <see cref="DateTimeOffset"/> from a buffer in <see cref="VolumeDescriptor"/> format.
		/// </summary>
		/// <param name="buffer">The buffer from which to read a <see cref="DateTimeOffset"/>.</param>
		/// <param name="offset">The offset in the buffer to read the <see cref="DateTimeOffset"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/></returns>
		DateTimeOffset ReadVolumeDescriptorDateTime(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var text = Encodings.ASCII.GetString(buffer, offset, 16).Replace('\0', '0');

			try
			{
				var year = Int32.Parse(text.Substring(0, 4));
				var month = Int32.Parse(text.Substring(4, 2));
				var day = Int32.Parse(text.Substring(6, 2));
				var hour = Int32.Parse(text.Substring(8, 2));
				var minute = Int32.Parse(text.Substring(10, 2));
				var second = Int32.Parse(text.Substring(12, 2));
				var hundredths = Int32.Parse(text.Substring(14, 2));
				var timeoffset = (SByte)buffer[offset + 16] * 15;

				var result = new DateTimeOffset(year, month, day, hour, minute, second, hundredths * 10, TimeSpan.FromMinutes(timeoffset));
				return result;
			}
			catch
			{
				return DateTimeOffset.MinValue;
			}
		}

		Byte[] FileSectorReadFunc(Int32 sector, Int32 count)
		{
			Reader.Position = sector * DefaultSectorSize;

			return Reader.ReadBytes(count);
		}

		#endregion

		#region ISO Writing

		void WriteFileSector(BinaryWriter writer, FileSector filesector)
		{
			Assert.IsNotNull(writer, nameof(writer));
			Assert.IsNotNull(filesector, nameof(filesector));

			writer.Write(filesector.GetData());
		}

		void WriteDirectoryRecordSector(BinaryWriter writer, DirectoryRecordSector sector)
		{
			Assert.IsNotNull(writer, nameof(writer));
			Assert.IsNotNull(sector, nameof(sector));

			var spaceleft = DefaultSectorSize;

			foreach (var record in sector.Records)
			{
				var buffer = new Byte[record.GetSize()];
				WriteDirectoryRecord(buffer, 0, record);

				if (spaceleft < buffer.Length)
				{
					writer.BaseStream.Position += spaceleft;
					spaceleft = DefaultSectorSize;
				}

				writer.Write(buffer);
				spaceleft -= buffer.Length;
			}
		}

		void WritePathTable(BinaryWriter writer, PathTable table)
		{
			Assert.IsNotNull(writer, nameof(writer));
			Assert.IsNotNull(table, nameof(table));

			foreach (var item in table.Items)
			{
				writer.Write((Byte)item.Name.Length);
				writer.Write((Byte)0);
				writer.Write(table.Endian == Endian.BigEndian ? BinaryIO.ChangeEndian(item.Sector) : item.Sector);
				writer.Write(table.Endian == Endian.BigEndian ? BinaryIO.ChangeEndian(item.ParentIndex) : item.ParentIndex);
				writer.Write(Encodings.ASCII.GetBytes(item.Name));

				if (item.Name.Length % 2 == 1) writer.Write((Byte)0);
			}
		}

		void WriteDirectoryRecord(Byte[] buffer, Int32 offset, DirectoryRecord record)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsNotNull(record, nameof(record));

			buffer[offset + 0] = record.GetSize();
			buffer[offset + 1] = record.ExtendedAttributeRecordLength;

			BinaryIO.WriteIntoBuffer(buffer, offset + 2, record.SectorNumber, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, offset + 6, record.SectorNumber, Endian.BigEndian);
			BinaryIO.WriteIntoBuffer(buffer, offset + 10, record.DataLength, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, offset + 14, record.DataLength, Endian.BigEndian);
			WriteDirectoryRecordDateTime(buffer, offset + 18, record.RecordingDateAndTime);

			buffer[offset + 25] = (Byte)record.Flags;
			buffer[offset + 26] = record.FileUnitSize;
			buffer[offset + 27] = record.InterleaveGapSize;

			BinaryIO.WriteIntoBuffer(buffer, offset + 28, record.VolumeSequenceNumber, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, offset + 30, record.VolumeSequenceNumber, Endian.BigEndian);

			buffer[offset + 32] = (Byte)record.FileIdentifier.Length;

			BinaryIO.WriteIntoBuffer(buffer, offset + 33, Encodings.ASCII, record.FileIdentifier, record.FileIdentifier.Length, ' ');

			var padding = record.FileIdentifier.Length % 2 == 0;

			if (record.SystemUseData != null) Array.Copy(record.SystemUseData, 0, buffer, offset + 33 + record.FileIdentifier.Length + (padding ? 1 : 0), record.SystemUseData.Length);
		}

		void WriteBasicVolumeDescriptor(BinaryWriter writer, BasicVolumeDescriptor descriptor)
		{
			Assert.IsNotNull(writer, nameof(writer));
			Assert.IsNotNull(descriptor, nameof(descriptor));

			var buffer = new Byte[DefaultSectorSize];

			buffer[0] = (Byte)descriptor.VolumeDescriptorType;

			BinaryIO.WriteIntoBuffer(buffer, 1, Encodings.ASCII, descriptor.StandardIdentifier, 5, ' ');

			buffer[6] = descriptor.VolumeDescriptorVersion;

			BinaryIO.WriteIntoBuffer(buffer, 8, Encodings.ASCII, descriptor.SystemIdentifier, 32, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 40, Encodings.ASCII, descriptor.VolumeIdentifier, 32, ' ');

			BinaryIO.WriteIntoBuffer(buffer, 80, descriptor.VolumeSpaceSize, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 84, descriptor.VolumeSpaceSize, Endian.BigEndian);

			BinaryIO.WriteIntoBuffer(buffer, 120, descriptor.VolumeSetSize, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 122, descriptor.VolumeSetSize, Endian.BigEndian);

			BinaryIO.WriteIntoBuffer(buffer, 124, descriptor.VolumeSequenceNumber, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 126, descriptor.VolumeSequenceNumber, Endian.BigEndian);

			BinaryIO.WriteIntoBuffer(buffer, 128, descriptor.LogicalBlockSize, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 130, descriptor.LogicalBlockSize, Endian.BigEndian);

			BinaryIO.WriteIntoBuffer(buffer, 132, descriptor.PathTableSize, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 136, descriptor.PathTableSize, Endian.BigEndian);

			BinaryIO.WriteIntoBuffer(buffer, 140, descriptor.TypeLPathTableLocation, Endian.LittleEndian);
			BinaryIO.WriteIntoBuffer(buffer, 144, descriptor.OptionalTypeLPathTableLocation, Endian.LittleEndian);

			BinaryIO.WriteIntoBuffer(buffer, 148, descriptor.TypeMPathTableLocation, Endian.BigEndian);
			BinaryIO.WriteIntoBuffer(buffer, 152, descriptor.OptionalTypeMPathTableLocation, Endian.BigEndian);

			WriteDirectoryRecord(buffer, 156, descriptor.RootDirectory);

			BinaryIO.WriteIntoBuffer(buffer, 190, Encodings.ASCII, descriptor.VolumeSetIdentifier, 128, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 318, Encodings.ASCII, descriptor.PublisherIdentifier, 128, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 446, Encodings.ASCII, descriptor.DataPreparerIdentifier, 128, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 574, Encodings.ASCII, descriptor.ApplicationIdentifier, 128, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 702, Encodings.ASCII, descriptor.CopyrightFileIdentifier, 37, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 739, Encodings.ASCII, descriptor.AbstractFileIdentifier, 37, ' ');
			BinaryIO.WriteIntoBuffer(buffer, 776, Encodings.ASCII, descriptor.BibliographicFileIdentifier, 37, ' ');

			WriteVolumeDescriptorDateTime(buffer, 813, descriptor.CreationDateAndTime);
			WriteVolumeDescriptorDateTime(buffer, 830, descriptor.ModificationDateAndTime);
			WriteVolumeDescriptorDateTime(buffer, 847, descriptor.ExpirationDateAndTime);
			WriteVolumeDescriptorDateTime(buffer, 864, descriptor.EffectiveDateAndTime);

			buffer[881] = descriptor.FileStructureVersion;

			writer.Write(buffer);
		}

		void WriteSetTerminatorVolumeDescriptor(BinaryWriter writer, SetTerminatorVolumeDescriptor descriptor)
		{
			Assert.IsNotNull(writer, nameof(writer));
			Assert.IsNotNull(descriptor, nameof(descriptor));

			var buffer = new Byte[DefaultSectorSize];

			buffer[0] = (Byte)descriptor.VolumeDescriptorType;

			BinaryIO.WriteIntoBuffer(buffer, 1, Encodings.ASCII, descriptor.StandardIdentifier, 5, ' ');

			buffer[6] = descriptor.VolumeDescriptorVersion;

			writer.Write(buffer);
		}

		void WriteVolumeDescriptorDateTime(Byte[] buffer, Int32 offset, DateTimeOffset time)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			if (time == DateTimeOffset.MinValue)
			{
				for (var i = 0; i != 16; ++i) buffer[offset + i] = (Byte)'0';
				buffer[offset + 16] = 0;

				return;
			}

			var str = time.ToString("yyyyMMddHHmmss");

			Encodings.ASCII.GetBytes(str, 0, str.Length, buffer, offset + 0);

			var hundredths = time.Millisecond / 10;
			var hundredthsstr = hundredths.ToString("00");

			Encodings.ASCII.GetBytes(hundredthsstr, 0, hundredthsstr.Length, buffer, offset + 14);

			buffer[offset + 16] = (Byte)(time.Offset.TotalMinutes / 15);
		}

		void WriteDirectoryRecordDateTime(Byte[] buffer, Int32 offset, DateTimeOffset time)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			buffer[offset + 0] = (Byte)(time.Year - 1900);
			buffer[offset + 1] = (Byte)(time.Month);
			buffer[offset + 2] = (Byte)(time.Day);
			buffer[offset + 3] = (Byte)(time.Hour);
			buffer[offset + 4] = (Byte)(time.Minute);
			buffer[offset + 5] = (Byte)(time.Second);
			buffer[offset + 6] = (Byte)(time.Offset.TotalMinutes / 15);
		}

		#endregion

		/// <summary>
		/// Contains all the <see cref="SectorObject"/>s in the image, mapped by the sector number of the image they are in.
		/// </summary>
		SortedDictionary<UInt32, SectorObject> SectorMap { get; }

		/// <summary>
		/// A mapping of a filesytem directory to the children of that directory.
		/// </summary>
		Dictionary<DirectoryRecord, List<DirectoryRecord>> DirectoryRecordTree { get; }

		/// <summary>
		/// <see cref="FileReader"/> used to read from the ISO9660 image file.
		/// </summary>
		FileReader Reader { get; }

		/// <summary>
		/// Default sector size for all ISO9660 images. This implementation of the spec assumes that the sector size of all images is the default.
		/// </summary>
		public static Int32 DefaultSectorSize { get; } = 2048;
	}
}