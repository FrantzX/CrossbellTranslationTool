using System;
using System.IO;
using System.Text;

namespace CrossbellTranslationTool
{
	class FileReader : IDisposable
	{
		public FileReader(Byte[] buffer, Encoding encoding)
		{
			Assert.IsNotNull(buffer, nameof(buffer));
			Assert.IsNotNull(encoding, nameof(encoding));

			Stream = new MemoryStream(buffer);
			Buffer = new Byte[256];
			StringBuilder = new StringBuilder(256);
			Encoding = encoding;
		}

		public FileReader(Stream stream, Encoding encoding)
		{
			Assert.IsNotNull(stream, nameof(stream));
			Assert.IsNotNull(encoding, nameof(encoding));

			Stream = stream;
			Buffer = new Byte[256];
			StringBuilder = new StringBuilder(256);
			Encoding = encoding;
		}

		public FileReader(String filepath, Encoding encoding)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(encoding, nameof(encoding));

			Stream = File.OpenRead(filepath);
			Buffer = new Byte[256];
			StringBuilder = new StringBuilder(256);
			Encoding = encoding;
		}

		public void Dispose()
		{
			Stream.Dispose();
		}

		public Int32 Peek()
		{
			var count = Stream.Read(Buffer, 0, 1);

			return (count == 1) ? Buffer[0] : -1;
		}

		public Byte[] ReadBytes(Int32 count)
		{
			var buffer = new Byte[count];

			Stream.Read(buffer, 0, count);

			return buffer;
		}

		public Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			return Stream.Read(buffer, offset, count);
		}

		public Byte ReadByte(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 1);

			return BinaryIO.ReadByteFromBuffer(Buffer, 0, endian);
		}

		public SByte ReadSByte(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 1);

			return BinaryIO.ReadSByteFromBuffer(Buffer, 0, endian);
		}

		public UInt16 ReadUInt16(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 2);

			return BinaryIO.ReadUInt16FromBuffer(Buffer, 0, endian);
		}

		public Int16 ReadInt16(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 2);

			return BinaryIO.ReadInt16FromBuffer(Buffer, 0, endian);
		}

		public UInt32 ReadUInt32(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 4);

			return BinaryIO.ReadUInt32FromBuffer(Buffer, 0, endian);
		}

		public Int32 ReadInt32(Endian endian = Endian.Default)
		{
			Stream.Read(Buffer, 0, 4);

			return BinaryIO.ReadInt32FromBuffer(Buffer, 0, endian);
		}

		public Text.FilePointer ReadFilePointer16()
		{
			var position = (UInt32)Stream.Position;
			var value = ReadUInt16();

			return new Text.FilePointer(Text.FilePointerSize.Size16, position, value);
		}

		public Text.FilePointer ReadFilePointer32()
		{
			var position = (UInt32)Stream.Position;
			var value = ReadUInt32();

			return new Text.FilePointer(Text.FilePointerSize.Size32, position, value);
		}

		public String ReadString()
		{
			StringBuilder.Length = 0;

			while (true)
			{
				var bytesread = Stream.Read(Buffer, 0, Buffer.Length);
				if (bytesread == 0) throw new EndOfStreamException();

				for (var i = 0; i < bytesread; ++i)
				{
					if (Buffer[i] == 0)
					{
						var rewindamount = bytesread - i - 1;
						Position -= rewindamount;

						return StringBuilder.ToString();
					}
					else if (Buffer[i] < 0x20)
					{
						if (Buffer[i] == (Byte)StringCode.COLOR)
						{
							if (i == Buffer.Length - 1)
							{
								--Position;
								break;
							}

							StringBuilder.Append((Char)Buffer[i]);
							StringBuilder.Append((Char)Buffer[i + 1]);

							++i;
						}
						else if (Buffer[i] == (Byte)StringCode.ITEM)
						{
							if (i >= Buffer.Length - 2)
							{
								Position -= 2;
								break;
							}

							var item = BinaryIO.ReadUInt16FromBuffer(Buffer, i + 1);

							StringBuilder.Append((Char)Buffer[i]);
							StringBuilder.Append((Char)item);

							i = i + 2;
						}
						else
						{
							StringBuilder.Append((Char)Buffer[i]);
						}
					}
					else if (Buffer[i] < 0x7F)
					{
						StringBuilder.Append((Char)Buffer[i]);
					}
					else
					{
						if (i == Buffer.Length - 1)
						{
							--Position;
							break;
						}

						var substring = Encoding.GetString(Buffer, i, 2);
						StringBuilder.Append(substring);

						++i;
					}
				}
			}
		}

		public Int64 Position
		{
			get { return Stream.Position; }
			set { Stream.Position = value; }
		}

		public Int64 Length
		{
			get { return Stream.Length; }
			set { Stream.SetLength(value); }
		}

		public Stream Stream { get; }

		Byte[] Buffer { get; }

		StringBuilder StringBuilder { get; }

		public Encoding Encoding { get; }
	}
}