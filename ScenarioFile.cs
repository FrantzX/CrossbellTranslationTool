using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace CrossbellTranslationTool
{
	class ScenarioFile
	{
		public ScenarioFile(FileReader reader, Type instructiontabletype)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(instructiontabletype, nameof(instructiontabletype));

			InstructionTableType = instructiontabletype;
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

			UpdateMainFunctionOffsets(FileMap.Values.OfType<List<UInt32>>().First(), offsetfixer);

			var placenames = FileMap.Values.OfType<List<FileHeaders.SCENARIO_PLACENAME>>().FirstOrDefault();
			if (placenames != null) UpdatePlaceNames(placenames, offsetfixer);

			UpdateBattle(FileMap, offsetfixer);

			VisitOperands(x => x.Type == Bytecode.OperandType.InstructionOffset, CreateOffsetWalker(offsetchanges, Bytecode.OperandType.InstructionOffset));
			VisitOperands(x => x.Type == Bytecode.OperandType.BattleOffset, CreateOffsetWalker(offsetchanges, Bytecode.OperandType.BattleOffset));

			QueueWorkItem2Hack();
		}

		public Byte[] Write(Encoding encoding)
		{
			Assert.IsNotNull(encoding, nameof(encoding));

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
					var bytes = EncodedStringUtil.GetBytes(str, encoding);
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
					instruction.Write(encoding, bytes, 0);
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

			return memstream.ToArray();
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

		List<List<Bytecode.Instruction>> GetAllInstructions()
		{
			var functionoffsets = FileMap.Values.OfType<List<UInt32>>().First();

			var output = new List<List<Bytecode.Instruction>>();

			for (var i = 0; i != functionoffsets.Count; ++i)
			{
				var startoffset = functionoffsets[i];
				var endoffset = (i + 1 < functionoffsets.Count) ? functionoffsets[i + 1] : Int32.MaxValue;

				var instructions = WalkInstructions((Int32)startoffset, (Int32)endoffset).ToList();

				output.Add(instructions);
			}

			return output;
		}

		IEnumerable<Bytecode.Instruction> WalkInstructions(Int32 startoffset, Int32 endoffset)
		{
			var instructionlist = new List<Bytecode.Instruction>();

			foreach (var item in FileMap.Where(x => x.Key >= startoffset && x.Key < endoffset))
			{
				if (item.Value is Bytecode.Instruction instruction)
				{
					yield return instruction;

					VisitOperands(instruction, x => x.Operands, o => o.Type == Bytecode.OperandType.Instruction, (root, list, index) => { instructionlist.Add((Bytecode.Instruction)list[index].Value); });

					foreach (var childinstruction in instructionlist) yield return childinstruction;
					instructionlist.Clear();
				}
			}
		}

		public void VisitOperands(Func<Bytecode.Operand, Boolean> filter, Action<Bytecode.Operand> visitor)
		{
			Assert.IsNotNull(filter, nameof(filter));
			Assert.IsNotNull(visitor, nameof(visitor));

			foreach (var instruction in FileMap.Values.OfType<Bytecode.Instruction>())
			{
				VisitOperands(instruction, x => x.Operands, filter, (root, list, index) => { visitor(list[index]); });
			}
		}

		public void VisitOperands(Func<Bytecode.Operand, Boolean> filter, Func<Bytecode.Operand, Bytecode.Operand> visitor)
		{
			Assert.IsNotNull(filter, nameof(filter));
			Assert.IsNotNull(visitor, nameof(visitor));

			foreach (var instruction in FileMap.Values.OfType<Bytecode.Instruction>())
			{
				VisitOperands(instruction, x => x.Operands, filter, (root, list, index) => { var item = visitor(list[index]); if (item != null) list[index] = item; });
			}
		}

		void VisitOperands<T>(T root, Func<T, List<Bytecode.Operand>> locator, Func<Bytecode.Operand, Boolean> filter, Action<Object, List<Bytecode.Operand>, Int32> callback)
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
					callback(root, list, i);
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

		public List<List<String>> GetFunctionStrings()
		{
			var instructions = GetAllInstructions();

			var output = instructions.Select(list => list.Select(instruction => instruction.Operands.Where(operand => operand.Type == Bytecode.OperandType.String)).SelectMany(x => x).Select(x => x.GetValue<String>()).ToList()).ToList();

			return output;
		}

		public void SetFunctionStrings(List<List<String>> values)
		{
			Assert.IsNotNull(values, nameof(values));

			var instructions = GetAllInstructions();

			foreach (var item in Enumerable.Zip(instructions, values, (x, y) => new { Instructions = x, Strings = y }))
			{
				var stringindex = 0;

				foreach (var instruction in item.Instructions)
				{
					for (var i = 0; i != instruction.Operands.Count; ++i)
					{
						if (instruction.Operands[i].Type == Bytecode.OperandType.String)
						{
							instruction.Operands[i] = new Bytecode.Operand(Bytecode.OperandType.String, item.Strings[stringindex++]);
						}
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
			var disassembler = new Bytecode.Disassembler(InstructionTableType);

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

		/// <summary>
		/// Has to be called after fixing jump offsets.
		/// </summary>
		void QueueWorkItem2Hack()
		{
			foreach (var item in FileMap.Where(x => x.Value is Bytecode.Instruction instruction && instruction.Definition == Bytecode.InstructionTable_AoKScena.QueueWorkItem2))
			{
				var queueworkitem2 = (Bytecode.Instruction)item.Value;
				var jump = queueworkitem2.Operands.Last().GetValue<Bytecode.Instruction>();

				jump.Operands[0] = new Bytecode.Operand(Bytecode.OperandType.InstructionOffset, item.Key + 5);
			}
		}

		static void UpdatePlaceNames(List<FileHeaders.SCENARIO_PLACENAME> placenames, Func<UInt32, UInt32> fixer)
		{
			Assert.IsNotNull(placenames, nameof(placenames));
			Assert.IsNotNull(fixer, nameof(fixer));

			for (var i = 0; i != placenames.Count; ++i)
			{
				var placename = placenames[i];

				placename.NameOffset = fixer(placename.NameOffset);

				placenames[i] = placename;
			}
		}

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

		public SortedDictionary<UInt32, Object> FileMap { get; }

		Type InstructionTableType { get; }
	}
}
