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