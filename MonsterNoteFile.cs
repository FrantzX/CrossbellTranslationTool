using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CrossbellTranslationTool
{
	class MonsterNoteFile
	{
		public class Record
		{
			public String Id { get; set; }

			public IMonsterDefinitionFile MonsterDefinitionFile { get; set; }

			public override String ToString()
			{
				return Id;
			}
		}

		public MonsterNoteFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			Records = new List<Record>();

			while (true)
			{
				var id = ConvertId(reader.ReadBytes(4));
				var length = reader.ReadUInt32();

				if (length == 0xFFFFFFFF) break;

				var filebytes = reader.ReadBytes((Int32)length);

				var monsterfile = OpenMonsterDefinitionFile(game, new FileReader(filebytes, reader.Encoding));

				Records.Add(new Record { Id = id, MonsterDefinitionFile = monsterfile });
			}
		}

		public Byte[] Write(Encoding encoding)
		{
			Assert.IsNotNull(encoding, nameof(encoding));

			using (var stream = new MemoryStream())
			{
				foreach (var record in Records)
				{
					var idbytes = ConvertId(record.Id);
					var filebytes = record.MonsterDefinitionFile.Write(encoding);

					var sizebytes = new Byte[4];
					BinaryIO.WriteIntoBuffer(sizebytes, 0, (Int32)filebytes.Length);

					stream.Write(idbytes, 0, idbytes.Length);
					stream.Write(sizebytes, 0, sizebytes.Length);
					stream.Write(filebytes, 0, filebytes.Length);
				}

				stream.Write(new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0, 8);

				return stream.ToArray();
			}
		}

		static String ConvertId(Byte[] bytes)
		{
			Assert.IsNotNull(bytes, nameof(bytes));
			Assert.IsTrue(bytes.Length == 4, "bytes.Length == 4");

			var id = $"{bytes[3]:X2}{bytes[2]:X2}{bytes[1]:X2}{bytes[0]:X2}";
			return id;
		}

		static Byte[] ConvertId(String id)
		{
			Assert.IsValidString(id, nameof(id));
			Assert.IsTrue(id.Length == 8, "id.Length == 8");

			var bytes = new Byte[4];
			bytes[0] = Byte.Parse(id.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			bytes[1] = Byte.Parse(id.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			bytes[2] = Byte.Parse(id.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			bytes[3] = Byte.Parse(id.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

			return bytes;
		}

		static IMonsterDefinitionFile OpenMonsterDefinitionFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			switch (game)
			{
				case Game.Ao:
					return new MonsterDefinitionFile_Ao(reader);

				case Game.Zero:
					return new MonsterDefinitionFile_Zero(reader);

				default:
					throw new Exception();
			}
		}

		public List<Record> Records { get; }
	}
}
