using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CrossbellTranslationTool.Bytecode
{
	class DisassemblyState
	{
		public DisassemblyState(FileReader reader, Instruction instruction, Func<DisassemblyState, Expression> expressionreader, Func<FileReader, DisassemblyState> instructionreader, Func<FileReader, UInt32?, List<DisassemblyState>> instructionblockreader)
		{
			Assert.IsNotNull(reader, nameof(reader));
			Assert.IsNotNull(instruction, nameof(instruction));
			Assert.IsNotNull(expressionreader, nameof(expressionreader));
			Assert.IsNotNull(instructionreader, nameof(instructionreader));
			Assert.IsNotNull(instructionblockreader, nameof(instructionblockreader));

			Reader = reader;
			Instruction = instruction;
			StartPosition = (UInt32)reader.Position - 1; //account for opcode

			ExpressionReader = expressionreader;
			InstructionReader = instructionreader;
			InstructionBlockReader = instructionblockreader;
		}

		public List<Operand> ReadOperands(IList<OperandType> types)
		{
			Assert.IsNotNull(types, nameof(types));

			return types.Select(x => ReadOperand(x)).ToList();
		}

		public List<Operand> ReadOperands(params OperandType[] types)
		{
			Assert.IsNotNull(types, nameof(types));

			return types.Select(x => ReadOperand(x)).ToList();
		}

		public Operand ReadOperand(OperandType type)
		{
			Assert.IsValidEnumeration(type, nameof(type), true);

			if (type == OperandType.Byte)
			{
				var value = Reader.ReadByte();

				return new Operand(type, value);
			}

			if (type == OperandType.SByte)
			{
				var value = Reader.ReadSByte();

				return new Operand(type, value);
			}

			if (type == OperandType.UInt16)
			{
				var value = Reader.ReadUInt16();

				return new Operand(type, value);
			}

			if (type == OperandType.Int16)
			{
				var value = Reader.ReadInt16();

				return new Operand(type, value);
			}

			if (type == OperandType.UInt32)
			{
				var value = Reader.ReadUInt32();

				return new Operand(type, value);
			}

			if (type == OperandType.Int32)
			{
				var value = Reader.ReadInt32();

				return new Operand(type, value);
			}

			if (type == OperandType.InstructionOffset)
			{
				var value = Reader.ReadUInt32();

				return new Operand(type, value);
			}

			if (type == OperandType.BattleOffset)
			{
				var value = Reader.ReadUInt32();

				return new Operand(type, value);
			}

			if (type == OperandType.String)
			{
				var value = Reader.ReadString();

				return new Operand(type, value);
			}

			if (type == OperandType.Expression)
			{
				var value = ExpressionReader(this);

				return new Operand(type, value);
			}

			if (type == OperandType.Instruction)
			{
				var value = InstructionReader(Reader);

				return new Operand(type, value.Instruction);
			}

			throw new Exception();
		}

		public List<DisassemblyState> ReadInstructionBlock(UInt32? blocklength = null)
		{
			return InstructionBlockReader(Reader, blocklength);
		}

		public FileReader Reader { get; }

		public Instruction Instruction { get; }

		public UInt32 StartPosition { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Func<DisassemblyState, Expression> ExpressionReader { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Func<FileReader, DisassemblyState> InstructionReader { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Func<FileReader, UInt32?, List<DisassemblyState>> InstructionBlockReader { get; }
	}
}