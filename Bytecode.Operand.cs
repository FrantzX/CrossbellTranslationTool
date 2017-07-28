using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Bytecode
{
	class Operand
	{
		public Operand(OperandType type, Object value)
		{
			Assert.IsValidEnumeration(type, nameof(type), true);
			Assert.IsNotNull(value, nameof(value));

			Type = type;
			Value = value;

			Validate();
		}

		void Validate()
		{
			switch (Type)
			{
				case OperandType.Byte:
					Assert.IsTrue(Value is Byte, "Operand value is not a Byte.");
					break;

				case OperandType.SByte:
					Assert.IsTrue(Value is SByte, "Operand value is not a SByte.");
					break;

				case OperandType.UInt16:
					Assert.IsTrue(Value is UInt16, "Operand value is not a UInt16.");
					break;

				case OperandType.Int16:
					Assert.IsTrue(Value is Int16, "Operand value is not a Int16.");
					break;

				case OperandType.UInt32:
					Assert.IsTrue(Value is UInt32, "Operand value is not a UInt32.");
					break;

				case OperandType.Int32:
					Assert.IsTrue(Value is Int32, "Operand value is not a Int32.");
					break;

				case OperandType.Instruction:
					Assert.IsTrue(Value is Instruction, "Operand value is not a Instruction.");
					break;

				case OperandType.Expression:
					Assert.IsTrue(Value is Expression, "Operand value is not a Expression.");
					break;

				case OperandType.Operation:
					Assert.IsTrue(Value is Operation, "Operand value is not a Operation.");
					break;

				case OperandType.String:
					Assert.IsTrue(Value is String, "Operand value is not a String.");
					break;

				case OperandType.InstructionOffset:
					Assert.IsTrue(Value is UInt32, "Operand value is not a InstructionOffset(UInt32).");
					break;

				case OperandType.BattleOffset:
					Assert.IsTrue(Value is UInt32, "Operand value is not a BattleOffset(UInt32).");
					break;

				case OperandType.None:
				default:
					throw new Exception();
			}
		}

		public override String ToString()
		{
			return Value.ToString();
		}

		public T GetValue<T>()
		{
			return (T)Value;
		}

		public Int32 GetSize()
		{
			switch (Type)
			{
				case OperandType.Byte:
				case OperandType.SByte:
					return 1;

				case OperandType.UInt16:
				case OperandType.Int16:
					return 2;

				case OperandType.UInt32:
				case OperandType.Int32:
				case OperandType.InstructionOffset:
				case OperandType.BattleOffset:
					return 4;

				case OperandType.Instruction:
					return GetValue<Instruction>().GetSize();

				case OperandType.Expression:
					return GetValue<Expression>().GetSize();

				case OperandType.Operation:
					return GetValue<Operation>().GetSize();

				case OperandType.String:
					return EncodedStringUtil.GetSize(GetValue<String>());

				case OperandType.None:
				default:
					throw new Exception();
			}
		}

		public Int32 Write(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			switch (Type)
			{
				case OperandType.Byte:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<Byte>());
					return 1;

				case OperandType.SByte:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<SByte>());
					return 1;

				case OperandType.UInt16:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<UInt16>());
					return 2;

				case OperandType.Int16:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<Int16>());
					return 2;

				case OperandType.UInt32:
				case OperandType.InstructionOffset:
				case OperandType.BattleOffset:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<UInt32>());
					return 4;

				case OperandType.Int32:
					BinaryIO.WriteIntoBuffer(buffer, offset, GetValue<Int32>());
					return 4;

				case OperandType.Instruction:
					return GetValue<Instruction>().Write(buffer, offset);

				case OperandType.Expression:
					return GetValue<Expression>().Write(buffer, offset);

				case OperandType.Operation:
					return GetValue<Operation>().Write(buffer, offset);

				case OperandType.String:
					var bytes = EncodedStringUtil.GetBytes(GetValue<String>());
					Array.Copy(bytes, 0, buffer, offset, bytes.Length);
					return bytes.Length;

				case OperandType.None:
				default:
					throw new Exception();
			}
		}

		public OperandType Type { get; }

		public Object Value { get; }
	}
}