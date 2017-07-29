using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrossbellTranslationTool
{
	static class BuildAction
	{
		public static void Run(CommandLine.BuildArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			Console.WriteLine("Building ISO");
			Console.WriteLine($"Source ISO: {args.SourceIsoPath}");
			Console.WriteLine($"Destination ISO: {args.DestinationIsoPath}");
			Console.WriteLine($"Data Location: {args.DataPath}");
			Console.WriteLine();

			var data_text = Text.TextFileDescription.GetTextFileData();
			var data_scenario = ScenarioFileList.GetList();

			using (var iso = new Iso9660.IsoImage(args.SourceIsoPath))
			{
				if (File.Exists(Path.Combine(args.DataPath, "EBOOT.BIN")) == true)
				{
					Console.WriteLine(@"EBOOT.BIN");

					var patches = BuildEbootPatches();
					var file = iso.GetFile(@"PSP_GAME\SYSDIR\EBOOT.BIN");
					var buffer = File.ReadAllBytes(Path.Combine(args.DataPath, "EBOOT.BIN"));

					foreach (var patch in patches)
					{
						if (patch.Type == BinaryPatchType.Clear)
						{
							for (var i = 0; i != patch.Count; ++i) buffer[patch.Offset + i] = 0;
						}

						if (patch.Type == BinaryPatchType.Overwrite)
						{
							if (patch.Buffer != null)
							{
								Array.Copy(patch.Buffer, 0, buffer, patch.Offset, patch.Buffer.Length);
							}

							if (patch.Text != null)
							{
								var stringbytes = EncodedStringUtil.GetBytes(patch.Text);
								Array.Copy(stringbytes, 0, buffer, patch.Offset, stringbytes.Length);
							}
						}
					}

					UpdateFileData(iso, file, buffer);
				}

				var stringtableitems = JsonTextItemFileIO.ReadFromFile(Path.Combine(args.DataPath, "stringtable.json"));

				foreach (var item in data_text)
				{
					var textfilepath = Path.Combine(IsoFilePaths.DirectoryPath_Text, item.FileName);
					var jsonfilepath = Path.ChangeExtension(Path.Combine(args.DataPath, "text", item.FileName), ".json");

					if (File.Exists(jsonfilepath) == true)
					{
						Console.WriteLine(item.FileName);
						UpdateTextFile(iso, textfilepath, item.FilePointerDelegate, jsonfilepath);
					}
				}

				foreach (var item in data_scenario)
				{
					var scenariofilepath = Path.Combine(IsoFilePaths.DirectoryPath_Scenario, item);
					var jsonfilepath = Path.ChangeExtension(Path.Combine(args.DataPath, "scena", item), ".json");

					if (File.Exists(jsonfilepath) == true)
					{
						Console.WriteLine(item);
						UpdateScenarioFile(iso, scenariofilepath, jsonfilepath, stringtableitems);
					}
				}

				CleanAllMC1Files(iso);

				foreach (var item in iso.GetChildren(IsoFilePaths.DirectoryPath_BattleData).Where(x => x.FileIdentifier.StartsWith("ms") == true))
				{
					var filepath = Path.Combine(IsoFilePaths.DirectoryPath_BattleData, item.FileIdentifier);
					var jsonfilepath = Path.ChangeExtension(Path.Combine(args.DataPath, "monster", item.FileIdentifier), ".json");

					if (File.Exists(jsonfilepath) == true)
					{
						Console.WriteLine(item.FileIdentifier);
						UpdateMonsterFile(iso, filepath, jsonfilepath);
					}
				}

				UpdateMonsterNote(iso, IsoFilePaths.FilePath_monsnotedt2);

				iso.GetPrimaryVolumeDescriptor().VolumeSpaceSize = iso.GetHighestSectorUsed() + 1;

				Console.WriteLine();
				Console.WriteLine("Writing ISO.");

				iso.Save(args.DestinationIsoPath);
			}

			Console.WriteLine("Done.");
		}

		static List<BinaryPatch> BuildEbootPatches()
		{
			var list = new List<BinaryPatch>();

			list.Add(new BinaryPatch { Type = BinaryPatchType.Clear, Offset = 0x28DC80, Count = 6 });
			list.Add(new BinaryPatch { Type = BinaryPatchType.Overwrite, Offset = 0x28DC80, Text = "Noel" });

			list.Add(new BinaryPatch { Type = BinaryPatchType.Clear, Offset = 0x2900C4, Count = 6 });
			list.Add(new BinaryPatch { Type = BinaryPatchType.Overwrite, Offset = 0x2900C4, Text = "Noel" });

			return list;
		}

		static void UpdateMonsterNote(Iso9660.IsoImage iso, String filepath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));

			Console.WriteLine(Path.GetFileName(filepath));

			var file = iso.GetFile(filepath);
			var filelist = GetMonsterNoteFileList();

			var allbuffers = new List<Byte[]>();

			foreach (var monsterfilenumber in filelist)
			{
				var monsterfilepath = Path.Combine(IsoFilePaths.DirectoryPath_BattleData, "ms" + monsterfilenumber + ".dat");
				var monsterfile = iso.GetFile(monsterfilepath);
				var monsterfiledata = monsterfile.GetData();

				var buffer = new Byte[8 + monsterfile.Record.DataLength];

				var foo = "300" + monsterfilenumber;
				var foobytes = Enumerable.Range(0, 4).Select(i => foo.Substring(i * 2, 2)).Select(str => (Byte)Int32.Parse(str, System.Globalization.NumberStyles.HexNumber)).Reverse().ToArray();

				Array.Copy(foobytes, 0, buffer, 0, 4);
				BinaryIO.WriteIntoBuffer(buffer, 4, monsterfile.Record.DataLength);
				Array.Copy(monsterfiledata, 0, buffer, 8, monsterfiledata.Length);

				allbuffers.Add(buffer);
			}

			allbuffers.Add(new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

			var totalsize = allbuffers.Sum(x => x.Length);
			var filebuffer = new Byte[totalsize];

			var memorystream = new MemoryStream(filebuffer);
			foreach (var item in allbuffers) memorystream.Write(item, 0, item.Length);

			UpdateFileData(iso, file, filebuffer);
			ClearFileReference(iso, IsoFilePaths.FilePath_sysonmembbc, Path.GetFileName(filepath));
		}

		static List<String> GetMonsterNoteFileList()
		{
			var list = new List<String>()
			{
				"02102",
				"02401",
				"03300",
				"03301",
				"03400",
				"03401",
				"03500",
				"03600",
				"03700",
				"03800",
				"03900",
				"04200",
				"24100",
				"31200",
				"31300",
				"32000",
				"32001",
				"32100",
				"32101",
				"41400",
				"41401",
				"41500",
				"41501",
				"41900",
				"41901",
				"41902",
				"42000",
				"42001",
				"42002",
				"42100",
				"42200",
				"42300",
				"42500",
				"43100",
				"43101",
				"43200",
				"43300",
				"44900",
				"60500",
				"60701",
				"60900",
				"61000",
				"61100",
				"61300",
				"61400",
				"61500",
				"61800",
				"62001",
				"62100",
				"62200",
				"62300",
				"62400",
				"62500",
				"62600",
				"62700",
				"62800",
				"63000",
				"63100",
				"63200",
				"63300",
				"63400",
				"63500",
				"63600",
				"63700",
				"63701",
				"63800",
				"63900",
				"64000",
				"64100",
				"64200",
				"64300",
				"64400",
				"64500",
				"64600",
				"64900",
				"65000",
				"65100",
				"65200",
				"65300",
				"65500",
				"65600",
				"65700",
				"65800",
				"65900",
				"66100",
				"66200",
				"66300",
				"66400",
				"66401",
				"66402",
				"66403",
				"66500",
				"66600",
				"66700",
				"66800",
				"66801",
				"66900",
				"67000",
				"67200",
				"67400",
				"68100",
				"68500",
				"68600",
				"68700",
				"68800",
				"68900",
				"69001",
				"69100",
				"69300",
				"69400",
				"69500",
				"69700",
				"69800",
				"69900",
				"70000",
				"70100",
				"70200",
				"70201",
				"70300",
				"70400",
				"70500",
				"70700",
				"70800",
				"71300",
				"71500",
				"71600",
				"71700",
				"71800",
				"71801",
				"71900",
				"72200",
				"72201",
				"72300",
				"72400",
				"72401",
				"72700",
				"72800",
				"73000",
				"73200",
				"73400",
				"73500",
				"73600",
				"73700",
				"74000",
				"74200",
				"74201",
				"74300",
				"74400",
				"74500",
				"74600",
				"74700",
				"74800",
				"75100",
				"75200",
				"75600",
				"75800",
				"75900",
				"76001",
				"76100",
				"76201",
				"77400",
				"78000",
				"78001",
				"78100",
				"78200",
				"78300",
				"78400",
				"78500",
				"78600",
				"78700",
				"78800",
				"78900",
				"79000",
				"79100",
				"79101",
				"79200",
				"79300",
				"79301",
				"79400",
				"79500",
				"79501",
				"79600",
				"79700",
				"79800",
				"79900",
				"80000",
				"80100",
				"80200",
				"80300",
				"80400",
				"80500",
				"80600",
				"80700",
				"80800",
				"80801",
				"80900",
				"81000",
				"81001",
				"81100",
				"81101",
				"81200",
				"81201",
				"81300",
				"81400",
				"81401",
				"81500",
				"81600",
				"81700",
				"81800",
				"81900",
				"82000",
				"82001",
				"82002",
				"82003",
				"82004",
				"82100",
				"82200",
				"82300",
				"82400",
				"82500",
				"82600",
				"82700",
				"82800",
				"82900",
				"83000",
				"83200",
				"83300",
				"83400",
				"83500",
				"83600",
				"83700",
				"83800",
				"83900",
				"84000",
				"84100",
				"84101",
				"84200",
				"84201",
				"84300",
				"84301",
				"84400",
				"84500",
				"84600",
				"84700",
				"84800",
				"84900",
				"85000",
				"85100",
				"85101",
				"85200",
				"85201",
				"85202",
				"85300",
				"85301",
				"85400",
				"85401",
				"85500",
				"85501",
				"85600",
				"85700",
				"85800",
				"85900",
				"86100",
				"86101",
				"86200",
				"86400",
				"86500",
				"86600",
				"86700",
				"86800",
				"86900",
				"87000",
				"87100",
				"87200",
				"87300",
				"87400",
				"87500",
				"87600",
				"87700",
				"87800",
				"87900",
				"88000",
				"88100",
				"88101",
				"88200",
				"88300",
				"88301",
				"88400",
				"88401",
				"88500",
				"88600",
				"88700",
				"88701",
				"88702",
				"88800",
				"88801",
				"88802",
				"88900",
				"88901",
				"89000",
				"89100",
				"89200",
				"89300",
				"89301",
				"89302"
			};

			return list;
		}

		static void UpdateMonsterFile(Iso9660.IsoImage iso, String filepath, String jsonpath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			var file = iso.GetFile(filepath);
			var reader = new FileReader(file.GetData());
			var monsterfile = new MonsterDefinitionFile(reader);

			var json = JsonTextItemFileIO.ReadFromFile(jsonpath);
			var strings = json.Select(x => x.GetBestText()).ToList();

			monsterfile.SetStrings(strings);

#warning Hack.
			var buffer = (monsterfile.SaveToStream() as MemoryStream).ToArray();

			UpdateFileData(iso, file, buffer);

			ClearFileReference(iso, IsoFilePaths.FilePath_btasm1bbc, Path.GetFileName(filepath));
		}

		static void UpdateTextFile(Iso9660.IsoImage iso, String filepath, Text.FilePointerDelegate filepointerfunc, String jsonpath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			WriteTextFile(iso, filepath, filepointerfunc, jsonpath);
			Update_datalst(iso, filepath);
			ClearFileReference(iso, IsoFilePaths.FilePath_sysstartbbc, Path.GetFileName(filepath));
			ClearFileReference(iso, IsoFilePaths.FilePath_sysonmembbc, Path.GetFileName(filepath));
		}

		static void UpdateScenarioFile(Iso9660.IsoImage iso, String scenariofilepath, String jsonpath, List<TextItem> stringtableitems)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(scenariofilepath, nameof(scenariofilepath));
			Assert.IsValidString(jsonpath, nameof(jsonpath));
			Assert.IsNotNull(stringtableitems, nameof(stringtableitems));

			var file = iso.GetFile(scenariofilepath);
			var scenariofile = new ScenarioFile(new FileReader(file.GetData()));

			var json = JsonTextItemFileIO.ReadFromFile(jsonpath);

			var scenariotext_index = 0;
			scenariofile.VisitOperands(x => x.Type == Bytecode.OperandType.String, x => new Bytecode.Operand(Bytecode.OperandType.String, json[scenariotext_index++].GetBestText()));

			var stringtable = scenariofile.GetStringTable();

			for (var i = 0; i != stringtable.Count; ++i)
			{
				var textitem = stringtableitems.FirstOrDefault(x => x.Text == stringtable[i]);
				if (textitem != null && textitem.Translation != "") stringtable[i] = textitem.Translation;
			}

			scenariofile.UpdateStringTable(stringtable);

			scenariofile.Fix();

#warning Hack.
			var buffer = (scenariofile.WriteToStream() as MemoryStream).ToArray();

			UpdateFileData(iso, file, buffer);

			Update_datalst(iso, scenariofilepath);
		}

		static void WriteTextFile(Iso9660.IsoImage iso, String filepath, Text.FilePointerDelegate filepointerfunc, String jsonpath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			var file = iso.GetFile(filepath);
			var sourcebytes = file.GetData();

			var textitems = JsonTextItemFileIO.ReadFromFile(jsonpath);
			var strings = textitems.Select(x => x.Translation).ToList();

			using (var stream = new MemoryStream(sourcebytes))
			using (var reader = new FileReader(stream))
			{
				var outputbytes = Text.TextFileIO.Write(reader, filepointerfunc, strings);
				UpdateFileData(iso, file, outputbytes);
			}
		}

		static void Update_datalst(Iso9660.IsoImage iso, String filepath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));

			var datalist = iso.GetFile(IsoFilePaths.FilePath_datalst);
			var datalistbuffer = datalist.GetData();
			var extensionslist = ReadDataLstExtensions(new FileReader(datalistbuffer));

			var filerecord = iso.GetFile(filepath);

			var index = FindDataLstIndex(filepath, datalistbuffer, extensionslist);

			BinaryIO.WriteIntoBuffer(datalistbuffer, index + 8, filerecord.Record.DataLength, Endian.LittleEndian);

			var sectorbytes = new Byte[4];
			BinaryIO.WriteIntoBuffer(sectorbytes, 0, filerecord.Record.SectorNumber, Endian.LittleEndian);

			for (var i = 0; i != 3; ++i) datalistbuffer[index + 12 + i] = sectorbytes[i];
		}

		static Int32 FindDataLstIndex(String filepath, Byte[] datalstbuffer, List<String> extensions)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(datalstbuffer, nameof(datalstbuffer));
			Assert.IsNotNull(extensions, nameof(extensions));

			var filename = Path.GetFileNameWithoutExtension(filepath);
			var ext = Path.GetExtension(filepath).Substring(1);
			var extensionindex = extensions.IndexOf(ext);

			var namebytes = System.Text.Encoding.ASCII.GetBytes(filename);

			var index = -1;
			while (true)
			{
				index = FindIndex(datalstbuffer, index + 1, namebytes);
				if (index == -1) return -1;

				if (datalstbuffer[index + 0xF] == extensionindex) return index;
			}
		}

		static void ClearFileReference(Iso9660.IsoImage iso, String filepath, String key)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsValidString(key, nameof(key));

			var file = iso.GetFile(filepath);
			var filebuffer = file.GetData();

			var keybytes = Encodings.ASCII.GetBytes(key);

			var find_index = FindIndex(filebuffer, 0, keybytes);
			if (find_index == -1) return;

			for (var i = 0; i != 32; ++i) filebuffer[find_index + i] = 0;
		}

		static void CleanAllMC1Files(Iso9660.IsoImage iso)
		{
			Assert.IsNotNull(iso, nameof(iso));

			var children = iso.GetChildren(IsoFilePaths.DirectoryPath_Map4).Where(x => Path.GetExtension(x.FileIdentifier) == ".mc1").ToList();

			foreach (var child in children)
			{
				CleanMC1File(iso, Path.Combine(IsoFilePaths.DirectoryPath_Map4, child.FileIdentifier));
			}
		}

		static void CleanMC1File(Iso9660.IsoImage iso, String filepath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));

			var file = iso.GetFile(filepath);
			var buffer = file.GetData();

			var count = BitConverter.ToUInt32(buffer, 0);

			for (var i = 0; i != count; ++i)
			{
				var offset = 16 + (i * 32);

				var key = System.Text.Encoding.ASCII.GetString(buffer, offset, 16).Trim('\0');

				if (Path.GetExtension(key) == ".bin")
				{
					for (var ii = 0; ii != 32; ++ii) buffer[offset + ii] = 0;
				}
			}
		}

		static List<String> ReadDataLstExtensions(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			var header = reader.ReadUInt32();

			var list = Enumerable.Range(0, 255).Select(x => reader.ReadBytes(4)).Select(x => Encodings.ASCII.GetString(x).Trim('\0')).ToList();
			list.Insert(0, "");

			return list;
		}

		static void UpdateFileData(Iso9660.IsoImage iso, Iso9660.FileSector file, Byte[] buffer)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsNotNull(file, nameof(file));
			Assert.IsNotNull(buffer, nameof(buffer));

			file.Record.DataLength = (UInt32)buffer.Length;
			file.SetData(buffer);

			var sectorsused = MathUtil.RoundUp(buffer.Length, 2048) / 2048;
			if (iso.CheckForRoom(file.Record, sectorsused) == false) iso.ChangeFileSector(file, iso.GetHighestSectorUsed() + 1);
		}

		static Int32 FindIndex(Byte[] buffer, Int32 offset, Byte[] item)
		{
			for (var i = offset; i != buffer.Length; ++i)
			{
				var match = true;

				for (var j = 0; j != item.Length; ++j)
				{
					if (i + j >= buffer.Length || buffer[i + j] != item[j]) { match = false; break; }
				}

				if (match) return i;
			}

			return -1;
		}
	}
}