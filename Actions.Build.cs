using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CrossbellTranslationTool.Actions
{
	static class Build
	{
		public static void Run(CommandLine.BuildArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			if (ValidateArgs(args) == false) return;

			if (args.Format == GameFormat.PSP)
			{
				Console.WriteLine("Building ISO");
				Console.WriteLine($"Source ISO: {args.SourceIsoPath}");
				Console.WriteLine($"Destination ISO: {args.DestinationIsoPath}");
				Console.WriteLine($"Translation Location: {args.TranslationPath}");
				Console.WriteLine();

				using (var iso = new Iso9660.IsoImage(args.SourceIsoPath))
				{
					var filesystem = new IO.IsoFileSystem(iso, @"PSP_GAME\USRDIR");

					var datapath_eboot = Path.Combine(args.TranslationPath, "EBOOT.BIN");
					if (File.Exists(datapath_eboot) == true)
					{
						Console.WriteLine(@"EBOOT.BIN");

						UpdateEboot(iso, datapath_eboot);
					}

					Run(args.Game, GameFormat.PSP, filesystem, Encodings.ShiftJIS, args.TranslationPath);

					FixFileReferences(args.Game, iso, filesystem);

					iso.GetPrimaryVolumeDescriptor().VolumeSpaceSize = iso.GetHighestSectorUsed() + 1;

					Console.WriteLine();
					Console.WriteLine("Writing ISO.");

					iso.Save(args.DestinationIsoPath);
				}
			}

			if (args.Format == GameFormat.PC)
			{
				Console.WriteLine("Injecting Translation");
				Console.WriteLine($"Game Path: {args.GamePath}");
				Console.WriteLine($"Translation Location: {args.TranslationPath}");
				Console.WriteLine();

				var filesystem = new IO.DirectoryFileSystem(args.GamePath);

				Run(args.Game, GameFormat.PC, filesystem, Encodings.Chinese, args.TranslationPath);
			}
		}

		static Boolean ValidateArgs(CommandLine.BuildArgs args)
		{
			Assert.IsNotNull(args, nameof(args));

			if (args.Game != Game.Ao) return false;
			if (args.Format == GameFormat.None) return false;
			if (args.Format == GameFormat.PSP && (args.SourceIsoPath == "" || args.DestinationIsoPath == "")) return false;
			if (args.Format == GameFormat.PC && args.GamePath == "") return false;

			return true;
		}

		static void Run(Game game, GameFormat format, IO.IFileSystem filesystem, Encoding encoding, String datapath)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsValidEnumeration(format, nameof(format), true);
			Assert.IsNotNull(filesystem, nameof(filesystem));
			Assert.IsNotNull(encoding, nameof(encoding));
			Assert.IsValidString(datapath, nameof(datapath));

			var data_text = Text.TextFileDescription.GetTextFileData(game);

			var stringtableitems = JsonTextItemFileIO.ReadFromFile(Path.Combine(datapath, "stringtable.json"));

			foreach (var item in data_text)
			{
				var textfilepath = Path.Combine(@"data\text", item.FileName);
				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "text", item.FileName), ".json");

				if (File.Exists(jsonfilepath) == true)
				{
					Console.WriteLine(item.FileName);

					using (var reader = filesystem.OpenFile(textfilepath, encoding))
					{
						var buffer = UpdateTextFile(reader, item.FilePointerDelegate, item.RecordCount, jsonfilepath);
						filesystem.SaveFile(textfilepath, buffer);
					}
				}
			}

			foreach (var filepath in filesystem.GetChildren(@"data\scena", "*.bin"))
			{
				var filename = Path.GetFileName(filepath);

				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "scena", filename), ".json");

				if (File.Exists(jsonfilepath) == true)
				{
					Console.WriteLine(filename);

					using (var reader = filesystem.OpenFile(filepath, encoding))
					{
						var buffer = UpdateScenarioFile(game, reader, jsonfilepath, stringtableitems);
						filesystem.SaveFile(filepath, buffer);
					}
				}
			}

			foreach (var filepath in filesystem.GetChildren(@"data\battle\dat", "ms*.dat"))
			{
				var filename = Path.GetFileName(filepath);
				var jsonfilepath = Path.ChangeExtension(Path.Combine(datapath, "monster", filename), ".json");

				if (File.Exists(jsonfilepath) == true)
				{
					Console.WriteLine(filename);

					using (var reader = filesystem.OpenFile(filepath, encoding))
					{
						var buffer = UpdateMonsterFile(game, reader, jsonfilepath);
						filesystem.SaveFile(filepath, buffer);
					}
				}
			}

			UpdateMonsterNote(game, filesystem, encoding);
		}

		static void UpdateMonsterNote(Game game, IO.IFileSystem filesystem, Encoding encoding)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(filesystem, nameof(filesystem));
			Assert.IsNotNull(encoding, nameof(encoding));

			Console.WriteLine("monsnote.dt2");

			using (var reader = filesystem.OpenFile(@"data\monsnote\monsnote.dt2", encoding))
			{
				var monsternote = new MonsterNoteFile(game, reader);

				foreach (var record in monsternote.Records)
				{
					var filename = Path.Combine(@"data\battle\dat", "ms" + record.Id.Substring(3) + ".dat");

					using (var monsterfilereader = filesystem.OpenFile(filename, encoding))
					{
						var monsterfile = OpenMonsterDefinitionFile(game, monsterfilereader);
						record.MonsterDefinitionFile = monsterfile;
					}
				}

				var filebuffer = monsternote.Write(encoding);
				filesystem.SaveFile(@"data\monsnote\monsnote.dt2", filebuffer);
			}
		}

		static Byte[] UpdateMonsterFile(Game game, FileReader reader, String jsonpath)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			var monsterfile = OpenMonsterDefinitionFile(game, reader);

			var json = JsonTextItemFileIO.ReadFromFile(jsonpath);
			var strings = json.Select(x => x.GetBestText()).ToList();

			monsterfile.SetStrings(strings);

			return monsterfile.Write(reader.Encoding);
		}

		static Byte[] UpdateTextFile(FileReader reader, Text.FilePointerDelegate filepointerfunc, Int32 recordcount, String jsonpath)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(filepointerfunc, nameof(filepointerfunc));
			Assert.IsValidString(jsonpath, nameof(jsonpath));

			var textitems = JsonTextItemFileIO.ReadFromFile(jsonpath);
			var strings = textitems.Select(x => x.Translation).ToList();

			return Text.TextFileIO.Write(reader, filepointerfunc, recordcount, strings);
		}

		static Byte[] UpdateScenarioFile(Game game, FileReader reader, String jsonpath, List<TextItem> stringtableitems)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsValidString(jsonpath, nameof(jsonpath));
			Assert.IsNotNull(stringtableitems, nameof(stringtableitems));

			var scenariofile = OpenScenarioFile(game, reader);
			var textitems = JsonTextItemFileIO.ReadFromFile(jsonpath);

			var scenariotext_index = 0;
			scenariofile.VisitOperands(x => x.Type == Bytecode.OperandType.String, x => new Bytecode.Operand(Bytecode.OperandType.String, textitems[scenariotext_index++].GetBestText()));

			var stringtable = scenariofile.GetStringTable();

			for (var i = 0; i != stringtable.Count; ++i)
			{
				var textitem = stringtableitems.FirstOrDefault(x => x.Text == stringtable[i]);
				if (textitem != null && textitem.Translation != "") stringtable[i] = textitem.Translation;
			}

			scenariofile.UpdateStringTable(stringtable);

			scenariofile.Fix();

			return scenariofile.Write(reader.Encoding);
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

		static ScenarioFile OpenScenarioFile(Game game, FileReader reader)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(reader, nameof(reader));

			switch (game)
			{
				case Game.Ao:
					return new ScenarioFile(reader, typeof(Bytecode.InstructionTable_AoKScena));

				case Game.Zero:
					return new ScenarioFile(reader, typeof(Bytecode.InstructionTalble_ZoKScena));

				default:
					throw new Exception();
			}
		}

		#region PSP ISO Methods

		static void FixFileReferences(Game game, Iso9660.IsoImage iso, IO.IFileSystem filesystem)
		{
			Assert.IsValidEnumeration(game, nameof(game), true);
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsNotNull(filesystem, nameof(filesystem));

			var data_text = Text.TextFileDescription.GetTextFileData(game);

			foreach (var item in data_text)
			{
				var textfilepath = Path.Combine(IsoFilePaths.DirectoryPath_Text, item.FileName);

				Update_datalst(iso, textfilepath);
				ClearFileReference(iso, IsoFilePaths.FilePath_sysstartbbc, item.FileName);
				ClearFileReference(iso, IsoFilePaths.FilePath_sysonmembbc, item.FileName);

			}

			foreach (var item in filesystem.GetChildren(@"data\scena", "*.bin"))
			{
				var scenariofilepath = Path.Combine(@"PSP_GAME\USRDIR", item);

				Update_datalst(iso, scenariofilepath);
			}

			foreach (var filepath in filesystem.GetChildren(@"data\battle\dat", "ms*.dat"))
			{
				var filename = Path.GetFileName(filepath);

				var filepath2 = Path.Combine(@"PSP_GAME\USRDIR", filepath);

				Update_datalst(iso, filepath2);
				ClearFileReference(iso, IsoFilePaths.FilePath_btasm1bbc, filename);
			}

			CleanAllMC1Files(iso);

			Update_datalst(iso, IsoFilePaths.FilePath_monsnotedt2);
			ClearFileReference(iso, IsoFilePaths.FilePath_sysonmembbc, "monsnote.dt2");
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

		static void UpdateEboot(Iso9660.IsoImage iso, String ebootpath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(ebootpath, nameof(ebootpath));

			var patches = BuildEbootPatches();
			var file = iso.GetFile(@"PSP_GAME\SYSDIR\EBOOT.BIN");
			var buffer = File.ReadAllBytes(ebootpath);

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
						var stringbytes = EncodedStringUtil.GetBytes(patch.Text, Encodings.ShiftJIS);
						Array.Copy(stringbytes, 0, buffer, patch.Offset, stringbytes.Length);
					}
				}
			}

			UpdateFileData(iso, file, buffer);
		}

		static void Update_datalst(Iso9660.IsoImage iso, String filepath)
		{
			Assert.IsNotNull(iso, nameof(iso));
			Assert.IsValidString(filepath, nameof(filepath));

			var datalist = iso.GetFile(IsoFilePaths.FilePath_datalst);
			var datalistbuffer = datalist.GetData();
			var extensionslist = ReadDataLstExtensions(new FileReader(datalistbuffer, Encodings.ASCII));

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

		#endregion
	}
}