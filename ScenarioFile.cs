using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CrossbellTranslationTool
{
	class ScenarioFile
	{
		public ScenarioFile(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			FileMap = new SortedDictionary<UInt32, Object>();

			var header = Interop.ReadStructFromStream<FileHeaders.SCENARIO_HEADER>(reader.Stream);
			FileMap[0] = header;

			var chiplist = ReadScenarioInfoList<FileHeaders.SCENARIO_CHIP>(reader, header, 0);
			var npclist = ReadScenarioInfoList<FileHeaders.SCENARIO_NPC>(reader, header, 1);
			var monsterlist = ReadScenarioInfoList<FileHeaders.SCENARIO_MONSTER>(reader, header, 2);
			var eventlist = ReadScenarioInfoList<FileHeaders.SCENARIO_EVENT>(reader, header, 3);
			var actorlist = ReadScenarioInfoList<FileHeaders.SCENARIO_ACTOR>(reader, header, 4);
			var placenameinfolist = ReadList<FileHeaders.SCENARIO_PLACENAME>(reader, header.PlaceNameOffset, header.PlaceNameNumber);
			var scenafunctions = ReadList<UInt32>(reader, header.FunctionTableOffset, header.FunctionTableSize / 4);
			var chipframeinfo = ReadList<FileHeaders.SCENARIO_CHIPFRAMEINFO>(reader, header.ChipFrameInfoOffset, GetChipFrameCount(), FileHeaders.SCENARIO_CHIPFRAMEINFO.Read);
			var stringmap = ReadStringMap(reader, header);
			var functionmap = ReadFunctions(reader, scenafunctions);

			var battleoffsets = FindBattleOffsets();
			foreach (var battleoffset in battleoffsets)
			{
				if (battleoffset == 0) continue;
				if (battleoffset == UInt32.MaxValue) continue;

				reader.Stream.Position = battleoffset;

				ReadBattleInformation(reader);
			}
		}

		public void Fix()
		{
			var offsetchanges = CorrectOffsets();
			var offsetfixer = CreateOffsetReplacer(offsetchanges);

			FileMap[0] = UpdateHeader((FileHeaders.SCENARIO_HEADER)FileMap[0], offsetfixer);

			UpdateMainFunctionOffsets((List<UInt32>)FileMap.First(x => x.Value is List<UInt32>).Value, offsetfixer);

			UpdateBattle(FileMap, offsetfixer);

			VisitOperands(x => x.Type == Bytecode.OperandType.InstructionOffset, CreateOffsetWalker(offsetchanges, Bytecode.OperandType.InstructionOffset));
			VisitOperands(x => x.Type == Bytecode.OperandType.BattleOffset, CreateOffsetWalker(offsetchanges, Bytecode.OperandType.BattleOffset));
		}

		public Stream WriteToStream()
		{
			var memstream = new MemoryStream(1024);

			foreach (var item in FileMap)
			{
				memstream.Position = item.Key;

				if (item.Value is FileHeaders.SCENARIO_HEADER header)
				{
					var bytes = Interop.GetBytesOfStruct(ref header);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is List<FileHeaders.SCENARIO_CHIP> chiplist)
				{
					for (var i = 0; i != chiplist.Count; ++i)
					{
						var obj = chiplist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_NPC> npclist)
				{
					for (var i = 0; i != npclist.Count; ++i)
					{
						var obj = npclist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_MONSTER> monsterlist)
				{
					for (var i = 0; i != monsterlist.Count; ++i)
					{
						var obj = monsterlist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_EVENT> eventlist)
				{
					for (var i = 0; i != eventlist.Count; ++i)
					{
						var obj = eventlist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_ACTOR> actorlist)
				{
					for (var i = 0; i != actorlist.Count; ++i)
					{
						var obj = actorlist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_PLACENAME> placenamelist)
				{
					for (var i = 0; i != placenamelist.Count; ++i)
					{
						var obj = placenamelist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<FileHeaders.SCENARIO_CHIPFRAMEINFO> chipframelist)
				{
					for (var i = 0; i != chipframelist.Count; ++i)
					{
						var obj = chipframelist[i];
						var bytes = FileHeaders.SCENARIO_CHIPFRAMEINFO.Write(obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is List<UInt32> offsetlist)
				{
					for (var i = 0; i != offsetlist.Count; ++i)
					{
						var obj = offsetlist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is String str)
				{
					var bytes = EncodedStringUtil.GetBytes(str);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is FileHeaders.SCENARIO_BATTLE battle)
				{
					var bytes = Interop.GetBytesOfStruct(ref battle);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is FileHeaders.SCENARIO_BATTLEMONSTERINFO)
				{
					var obj = (FileHeaders.SCENARIO_BATTLEMONSTERINFO)item.Value;
					var bytes = Interop.GetBytesOfStruct(ref obj);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is List<FileHeaders.SCENARIO_BATTLEMONSTERPOSITION> positionlist)
				{
					for (var i = 0; i != positionlist.Count; ++i)
					{
						var obj = positionlist[i];
						var bytes = Interop.GetBytesOfStruct(ref obj);
						memstream.Write(bytes, 0, bytes.Length);
					}
				}
				else if (item.Value is Bytecode.Instruction instruction)
				{
					var bytes = new Byte[instruction.GetSize()];
					instruction.Write(bytes, 0);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is FileHeaders.SCENARIO_ATBONUS atbonus)
				{
					var bytes = Interop.GetBytesOfStruct(ref atbonus);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else if (item.Value is FileHeaders.SCENARIO_BATTLESEPITH sepith)
				{
					var bytes = Interop.GetBytesOfStruct(ref sepith);
					memstream.Write(bytes, 0, bytes.Length);
				}
				else
				{
					throw new Exception();
				}
			}

			memstream.Position = 0;
			return memstream;
		}

		public List<String> GetStringTable()
		{
			return FileMap.Values.OfType<String>().ToList();
		}

		public void UpdateStringTable(List<String> list)
		{
			Assert.IsNotNull(list, nameof(list));

			var stringoffsets = FileMap.Where(x => x.Value is String).Select(x => x.Key).ToList();

			for (var i = 0; i != stringoffsets.Count; ++i)
			{
				var offset = stringoffsets[i];
				FileMap[offset] = list[i];
			}
		}

		public void VisitOperands(Func<Bytecode.Operand, Boolean> filter, Action<Bytecode.Operand> visitor)
		{
			Assert.IsNotNull(filter, nameof(filter));
			Assert.IsNotNull(visitor, nameof(visitor));

			foreach (var instruction in FileMap.Values.OfType<Bytecode.Instruction>())
			{
				VisitOperands(instruction, x => x.Operands, filter, (list, index) => { visitor(list[index]); });
			}
		}

		public void VisitOperands(Func<Bytecode.Operand, Boolean> filter, Func<Bytecode.Operand, Bytecode.Operand> visitor)
		{
			Assert.IsNotNull(filter, nameof(filter));
			Assert.IsNotNull(visitor, nameof(visitor));

			foreach (var instruction in FileMap.Values.OfType<Bytecode.Instruction>())
			{
				VisitOperands(instruction, x => x.Operands, filter, (list, index) => { var item = visitor(list[index]); if (item != null) list[index] = item; });
			}
		}

		void VisitOperands<T>(T root, Func<T, List<Bytecode.Operand>> locator, Func<Bytecode.Operand, Boolean> filter, Action<List<Bytecode.Operand>, Int32> callback)
		{
			Assert.IsNotNull(root, nameof(root));
			Assert.IsNotNull(locator, nameof(locator));
			Assert.IsNotNull(filter, nameof(filter));
			Assert.IsNotNull(callback, nameof(callback));

			var list = locator(root);

			for (var i = 0; i != list.Count; ++i)
			{
				var operand = list[i];

				if (filter(operand) == true)
				{
					callback(list, i);
				}

				if (operand.Type == Bytecode.OperandType.Instruction)
				{
					VisitOperands(operand.GetValue<Bytecode.Instruction>(), x => x.Operands, filter, callback);
				}

				if (operand.Type == Bytecode.OperandType.Expression)
				{
					foreach (var operation in operand.GetValue<Bytecode.Expression>().Operations)
					{
						VisitOperands(operation, x => x.Operands, filter, callback);
					}
				}
			}

		}

		#region Reading

		Int32 GetChipFrameCount()
		{
			var monsterlist = FileMap.Values.OfType<List<FileHeaders.SCENARIO_MONSTER>>().FirstOrDefault();
			if (monsterlist == null) return 0;

			UInt32 count = 0;
			foreach (var monster in monsterlist)
			{
				count = Math.Max(count, monster.StandFrameInfoIndex);
				count = Math.Max(count, monster.MoveFrameInfoIndex);
			}

			if (count > 0) ++count;

			return (Int32)count;
		}

		HashSet<UInt32> FindBattleOffsets()
		{
			var battleoffsets = new HashSet<UInt32>();
			var monsterlist = FileMap.Values.OfType<List<FileHeaders.SCENARIO_MONSTER>>().FirstOrDefault();

			foreach (var instruction in FileMap.Values.OfType<Bytecode.Instruction>().Where(x => x.Definition == Bytecode.InstructionTable_AoKScena.Battle))
			{
				var battleoffset = instruction.Operands[0].GetValue<UInt32>();
				battleoffsets.Add(battleoffset);
			}

			if (monsterlist != null)
			{
				foreach (var monster in monsterlist)
				{
					battleoffsets.Add(monster.BattleInfoOffset);
				}
			}

			return battleoffsets;
		}

		void ReadBattleInformation(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			UInt32 offset = (UInt32)reader.Stream.Position;

			var battle = DefaultReadFunction<FileHeaders.SCENARIO_BATTLE>(reader);
			FileMap[offset] = battle;

			foreach (var val in battle.Probability)
			{
				if (val == 0) continue;

				var bmi_offset = (UInt32)reader.Stream.Position;
				var battlemonsterinfo = DefaultReadFunction<FileHeaders.SCENARIO_BATTLEMONSTERINFO>(reader);

				var positionnormaldata = ReadList<FileHeaders.SCENARIO_BATTLEMONSTERPOSITION>(reader, battlemonsterinfo.PositionNormalOffset, 8);
				var positionambushdata = ReadList<FileHeaders.SCENARIO_BATTLEMONSTERPOSITION>(reader, battlemonsterinfo.PositionEnemyAdvantageOffset, 8);

				FileHeaders.SCENARIO_ATBONUS atbonus;
				using (var saver = new StreamPositionSaver(reader.Stream))
				{
					reader.Stream.Position = battlemonsterinfo.ATBonusOffset;
					atbonus = DefaultReadFunction<FileHeaders.SCENARIO_ATBONUS>(reader);
				}

				FileMap[bmi_offset] = battlemonsterinfo;
				FileMap[battlemonsterinfo.PositionNormalOffset] = positionnormaldata;
				FileMap[battlemonsterinfo.PositionEnemyAdvantageOffset] = positionambushdata;
				FileMap[battlemonsterinfo.ATBonusOffset] = atbonus;
			}

			// Pointer into string table?
			//var battlemap = reader.ReadString(battle.BattleMapOffset);
			//filemap[battle.BattleMapOffset] = battlemap;

			if (battle.SepithOffset != 0)
			{
				reader.Stream.Position = battle.SepithOffset;
				var sepith = DefaultReadFunction<FileHeaders.SCENARIO_BATTLESEPITH>(reader);

				FileMap[battle.SepithOffset] = sepith;
			}
		}

		IDictionary<UInt32, Bytecode.Instruction> ReadFunctions(FileReader reader, List<UInt32> functionoffsets)
		{
			var instructionmap = new SortedDictionary<UInt32, Bytecode.Instruction>();
			var disassembler = new Bytecode.Disassembler(typeof(Bytecode.InstructionTable_AoKScena));

			foreach (var offset in functionoffsets)
			{
				reader.Stream.Position = offset;

				disassembler.Disassemble(reader, instructionmap);
			}

			foreach (var item in instructionmap) FileMap.Add(item.Key, item.Value);

			return instructionmap;
		}

		IDictionary<UInt32, String> ReadStringMap(FileReader reader, FileHeaders.SCENARIO_HEADER header)
		{
			var map = new SortedDictionary<UInt32, String>();

			reader.Stream.Position = header.StringTableOffset;

			while (true)
			{
				if (reader.Stream.Position == reader.Stream.Length) break;

				var offset = (UInt32)reader.Stream.Position;
				var str = reader.ReadString();

				map.Add(offset, str);
				FileMap.Add(offset, str);

				if (str == "") break;
			}

			return map;
		}

		List<T> ReadList<T>(FileReader reader, Int32 position, Int32 count, Func<FileReader, T> func = null) where T : struct
		{
			Assert.IsNotNull(reader, nameof(reader));

			func = func ?? DefaultReadFunction<T>;

			var list = new List<T>();

			using (var saver = new StreamPositionSaver(reader.Stream))
			{
				reader.Stream.Position = position;

				for (var i = 0; i != count; ++i) list.Add(func(reader));
			}

			if (list.Count > 0) FileMap[(UInt32)position] = list;

			return list;
		}

		List<T> ReadScenarioInfoList<T>(FileReader reader, FileHeaders.SCENARIO_HEADER header, Int32 index) where T : struct
		{
			return ReadList<T>(reader, header.ScnInfoOffset[index], header.ScnInfoNumber[index]);
		}

		T DefaultReadFunction<T>(FileReader reader) where T : struct
		{
			return Interop.ReadStructFromStream<T>(reader.Stream);
		}

		#endregion

		#region Fixing

		static void UpdateBattle(IDictionary<UInt32, Object> filemap, Func<UInt32, UInt32> fixer)
		{
			Assert.IsNotNull(filemap, nameof(filemap));
			Assert.IsNotNull(fixer, nameof(fixer));

			var battleoffsets = filemap.Where(x => x.Value is FileHeaders.SCENARIO_BATTLE).Select(x => x.Key).ToList();

			foreach (var offset in battleoffsets)
			{
				var battle = (FileHeaders.SCENARIO_BATTLE)filemap[offset];

				battle.BattleMapOffset = fixer(battle.BattleMapOffset);
				battle.SepithOffset = fixer(battle.SepithOffset);

				filemap[offset] = battle;
			}
		}

		static void UpdateMainFunctionOffsets(List<UInt32> list, Func<UInt32, UInt32> fixer)
		{
			Assert.IsNotNull(list, nameof(list));
			Assert.IsNotNull(fixer, nameof(fixer));

			for (var i = 0; i != list.Count; ++i) list[i] = fixer(list[i]);
		}

		static FileHeaders.SCENARIO_HEADER UpdateHeader(FileHeaders.SCENARIO_HEADER header, Func<UInt32, UInt32> fixer)
		{
			Assert.IsNotNull(fixer, nameof(fixer));

			header.StringTableOffset = fixer(header.StringTableOffset);
			header.FunctionTableOffset = (UInt16)fixer(header.FunctionTableOffset);

			return header;
		}

		IDictionary<UInt32, UInt32> CorrectOffsets()
		{
			var changes = new SortedDictionary<UInt32, UInt32>();

			var list = FileMap.ToList();
			var firstinstructioindex = list.FindIndex(x => x.Value is Bytecode.Instruction);

			var itemoffset = list[firstinstructioindex].Key;
			for (var i = firstinstructioindex; i >= 0 && i < list.Count; ++i)
			{
				var item = list[i];

				if (itemoffset != item.Key)
				{
					var diff = itemoffset - item.Key;

					list[i] = new KeyValuePair<UInt32, Object>(itemoffset, item.Value);
					changes[item.Key] = itemoffset;

#warning Hack
					if (item.Value is Bytecode.Instruction instruction && instruction.Definition == Bytecode.InstructionTable_AoKScena.QueueWorkItem2)
					{
						instruction.Operands.Last().GetValue<Bytecode.Instruction>().Operands[0] = new Bytecode.Operand(Bytecode.OperandType.InstructionOffset, itemoffset + 5);
					}
				}

				var size = (UInt32)GetObjectSize(item.Value);
				itemoffset += size;
			}

			FileMap.Clear();

			foreach (var item in list) FileMap.Add(item.Key, item.Value);

			return changes;
		}

		static Int32 GetObjectSize(Object value)
		{
			Assert.IsNotNull(value, nameof(value));

			if (value is Bytecode.Instruction instruction)
			{
				return instruction.GetSize();
			}

			if (value is String)
			{
				return EncodedStringUtil.GetSize((String)value);
			}

			if (value is FileHeaders.SCENARIO_BATTLESEPITH)
			{
				return System.Runtime.InteropServices.Marshal.SizeOf<FileHeaders.SCENARIO_BATTLESEPITH>();
			}

			throw new Exception();
		}

		static Func<UInt32, UInt32> CreateOffsetReplacer(IDictionary<UInt32, UInt32> fixmap)
		{
			Assert.IsNotNull(fixmap, nameof(fixmap));

			return offset =>
			{
				if (fixmap.TryGetValue(offset, out UInt32 newoffset) == true)
				{
					return newoffset;
				}
				else
				{
					return offset;
				}
			};
		}

		static Func<Bytecode.Operand, Bytecode.Operand> CreateOffsetWalker(IDictionary<UInt32, UInt32> fixmap, Bytecode.OperandType operandtype)
		{
			Assert.IsNotNull(fixmap, nameof(fixmap));
			Assert.IsValidEnumeration(operandtype, nameof(operandtype), true);

			return operand =>
			{
				if (fixmap.TryGetValue(operand.GetValue<UInt32>(), out UInt32 newoffset) == true)
				{
					return new Bytecode.Operand(operandtype, newoffset);
				}
				else
				{
					return null;
				}
			};
		}

		#endregion

		SortedDictionary<UInt32, Object> FileMap { get; }
	}
}
