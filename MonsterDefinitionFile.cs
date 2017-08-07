using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace CrossbellTranslationTool
{
	class MonsterDefinitionFile
	{
		class CraftInfo
		{
			public FileHeaders.MONSTER_CRAFTINFO Info { get; set; }

			public String Name { get; set; }

			public String Description { get; set; }
		}

		public MonsterDefinitionFile(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			Arts = new List<FileHeaders.MONSTER_CRAFTAIINFO>();
			Crafts = new List<FileHeaders.MONSTER_CRAFTAIINFO>();
			SCrafts = new List<FileHeaders.MONSTER_CRAFTAIINFO>();
			SupportCrafts = new List<FileHeaders.MONSTER_CRAFTAIINFO>();
			CraftInfoList = new List<CraftInfo>();

			var header = Interop.ReadStructFromStream<FileHeaders.MONSTER_HEADER>(reader.Stream);
			var attack = Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTAIINFO>(reader.Stream);
			var count_arts = reader.ReadByte();
			var arts = Enumerable.Range(0, count_arts).Select(i => Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTAIINFO>(reader.Stream)).ToList();
			var count_crafts = reader.ReadByte();
			var crafts = Enumerable.Range(0, count_crafts).Select(i => Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTAIINFO>(reader.Stream)).ToList();
			var count_scrafts = reader.ReadByte();
			var scrafts = Enumerable.Range(0, count_scrafts).Select(i => Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTAIINFO>(reader.Stream)).ToList();
			var count_supportcrafts = reader.ReadByte();
			var supportcrafts = Enumerable.Range(0, count_supportcrafts).Select(i => Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTAIINFO>(reader.Stream)).ToList();
			var count_craftinfo = reader.ReadByte();
			var craftinfo = Enumerable.Range(0, count_craftinfo).Select(i => new CraftInfo { Info = Interop.ReadStructFromStream<FileHeaders.MONSTER_CRAFTINFO>(reader.Stream), Name = reader.ReadString(), Description = reader.ReadString() }).ToList();
			var runaway = Interop.ReadStructFromStream<FileHeaders.MONSTER_RUNAWAY>(reader.Stream);
			var reserve1 = reader.ReadByte();
			var name = reader.ReadString();
			var description = reader.ReadString();

			Header = header;
			Attack = attack;
			Arts.AddRange(arts);
			Crafts.AddRange(crafts);
			SCrafts.AddRange(scrafts);
			SupportCrafts.AddRange(supportcrafts);
			CraftInfoList.AddRange(craftinfo);

			RunAway = runaway;
			Reserve = reserve1;
			Name = name;
			Description = description;
		}

		public Byte[] Write(Encoding encoding)
		{
			Assert.IsNotNull(encoding, nameof(encoding));

			var stream = new MemoryStream();

			Interop.WriteStructToStream(stream, Header);
			Interop.WriteStructToStream(stream, Attack);

			stream.WriteByte((Byte)Arts.Count);
			foreach(var info in Arts) Interop.WriteStructToStream(stream, info);

			stream.WriteByte((Byte)Crafts.Count);
			foreach (var info in Crafts) Interop.WriteStructToStream(stream, info);

			stream.WriteByte((Byte)SCrafts.Count);
			foreach (var info in SCrafts) Interop.WriteStructToStream(stream, info);

			stream.WriteByte((Byte)SupportCrafts.Count);
			foreach (var info in SupportCrafts) Interop.WriteStructToStream(stream, info);

			stream.WriteByte((Byte)CraftInfoList.Count);
			foreach (var info in CraftInfoList)
			{
				Interop.WriteStructToStream(stream, info.Info);
				EncodedStringUtil.WriteStringToStream(stream, info.Name, encoding);
				EncodedStringUtil.WriteStringToStream(stream, info.Description, encoding);
			}

			Interop.WriteStructToStream(stream, RunAway);
			stream.WriteByte(Reserve);

			EncodedStringUtil.WriteStringToStream(stream, Name, encoding);
			EncodedStringUtil.WriteStringToStream(stream, Description, encoding);

			return stream.ToArray();
		}

		public List<String> GetStrings()
		{
			var list = new List<String>();
			list.Add(Name);
			list.Add(Description);

			foreach (var info in CraftInfoList)
			{
				list.Add(info.Name);
				list.Add(info.Description);
			}

			return list;
		}

		public void SetStrings(List<String> list)
		{
			Assert.IsNotNull(list, nameof(list));

			Name = list[0];
			Description = list[1];

			for (var i = 0; i != CraftInfoList.Count; ++i)
			{
				var info = CraftInfoList[i];

				info.Name = list[(i * 2) + 2 + 0];
				info.Description = list[(i * 2) + 2 + 1];
			}
		}

		FileHeaders.MONSTER_HEADER Header { get; set; }

		FileHeaders.MONSTER_CRAFTAIINFO Attack { get; set; }

		List<FileHeaders.MONSTER_CRAFTAIINFO> Arts { get; }

		List<FileHeaders.MONSTER_CRAFTAIINFO> Crafts { get; }

		List<FileHeaders.MONSTER_CRAFTAIINFO> SCrafts { get; }

		List<FileHeaders.MONSTER_CRAFTAIINFO> SupportCrafts { get; }

		List<CraftInfo> CraftInfoList { get; }

		FileHeaders.MONSTER_RUNAWAY RunAway { get; set; }

		Byte Reserve { get; set; }

		String Name { get; set; }

		String Description { get; set; }
	}
}
