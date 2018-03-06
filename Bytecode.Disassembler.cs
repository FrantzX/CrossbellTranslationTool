using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Bytecode
{
	class Disassembler
	{
		public Disassembler(Type instructiontabletype)
		{
			Assert.IsNotNull(instructiontabletype, nameof(instructiontabletype));

			InstructionDefinitionMap = new Dictionary<Byte, InstructionDefinition>();

			BuildInstructionDefinitionMap(instructiontabletype);
		}

		void BuildInstructionDefinitionMap(Type instructiontabletype)
		{
			Assert.IsNotNull(instructiontabletype, nameof(instructiontabletype));

			InstructionDefinitionMap.Clear();

			var propertyinfos = instructiontabletype.GetProperties().Where(x => x.PropertyType == typeof(InstructionDefinition));

			foreach (var item in propertyinfos)
			{
				var instructiondefinition = (InstructionDefinition)item.GetValue(null);

				InstructionDefinitionMap.Add(instructiondefinition.OpCode, instructiondefinition);
			}
		}

		public IDictionary<UInt32, Instruction> Disassemble(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			var instructionmap = new SortedDictionary<UInt32, Instruction>();

			Disassemble(reader, instructionmap);

			return instructionmap;
		}

		public void Disassemble(FileReader reader, IDictionary<UInt32, Instruction> instructionmap)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(instructionmap, nameof(instructionmap));

			var instruction_offset = (UInt32)reader.Position;
			if (instructionmap.ContainsKey(instruction_offset) == true) return;

			var instructionblock = ReadInstructionBlock(reader);

			var referencedoffsets = new List<UInt32>();

			foreach (var item in instructionblock)
			{
				if (instructionmap.ContainsKey(item.StartPosition) == true) continue;
				instructionmap.Add(item.StartPosition, item.Instruction);

				foreach(var operand in item.Instruction.Operands)
				{
					if (operand.Type == OperandType.InstructionOffset) referencedoffsets.Add(operand.GetValue<UInt32>());
				}
			}

			foreach (var reference_offset in referencedoffsets)
			{
				if (reference_offset == 0) continue;

				//hack for bad jump in ZoK ENG c011b.bin
				if (reader.Length == 104091 && reference_offset == 92452) continue;

				//hack for bad jump in ZoK ENG m3033.bin
				if (reader.Length == 5191 && reference_offset == 4961) continue;

				reader.Position = reference_offset;
				Disassemble(reader, instructionmap);
			}
		}

		List<DisassemblyState> ReadInstructionBlock(FileReader reader, UInt32? blocklength = null)
		{
			Assert.IsNotNull(reader, nameof(reader));

			var list = new List<DisassemblyState>();

			var endofblock = blocklength.HasValue ? reader.Position + blocklength.Value : reader.Length;

			while (reader.Position < endofblock)
			{
				var parsestate = ReadInstruction(reader);

				list.Add(parsestate);

				if ((parsestate.Instruction.Definition.Flags & InstructionFlags.EndBlock) == InstructionFlags.EndBlock) break;
			}

			return list;
		}

		DisassemblyState ReadInstruction(FileReader reader)
		{
			Assert.IsNotNull(reader, nameof(reader));

			var opcode = reader.ReadByte();
			var instructiondefinition = InstructionDefinitionMap[opcode];
			var instruction = new Instruction(instructiondefinition);

			var state = new DisassemblyState(reader, instruction, ReadExpression, ReadInstruction, ReadInstructionBlock);
			var builderfunction = instructiondefinition.DisassemblyFunction;
			builderfunction(state);

			return state;
		}

		Expression ReadExpression(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var expression = new Expression();

			while (true)
			{
				var operationtype = (OperationType)state.Reader.ReadByte();
				var operation = new Operation(operationtype);

				expression.Operations.Add(operation);

				if (operationtype == OperationType.PUSH_LONG)
				{
					operation.Operands.Add(state.ReadOperand(OperandType.UInt32));
				}
				else if (operationtype == OperationType.EXEC_OP)
				{
					operation.Operands.Add(state.ReadOperand(OperandType.Instruction));
				}
				else if (operationtype == OperationType.TEST_SCENA_FLAGS || operationtype == OperationType.GET_RESULT)
				{
					operation.Operands.Add(state.ReadOperand(OperandType.UInt16));
				}
				else if (operationtype == OperationType.PUSH_VALUE_INDEX || operationtype == OperationType.UNKNOWN_23)
				{
					operation.Operands.Add(state.ReadOperand(OperandType.Byte));
				}
				else if (operationtype == OperationType.GET_CHR_WORK)
				{
					operation.Operands.Add(state.ReadOperand(OperandType.UInt16));
					operation.Operands.Add(state.ReadOperand(OperandType.Byte));
				}
				else if (operationtype == OperationType.END)
				{
					break;
				}
			}

			return expression;
		}

		Dictionary<Byte, InstructionDefinition> InstructionDefinitionMap { get; }
	}
}