using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Bytecode
{
	class Instruction
	{
		public Instruction(InstructionDefinition definition)
		{
			Assert.IsNotNull(definition, nameof(definition));

			Definition = definition;
			Operands = new List<Operand>();
		}

		public override String ToString()
		{
			var builder = new System.Text.StringBuilder();

			builder.Append(Definition.Name);

			if (Operands.Count > 0)
			{
				builder.Append("(");
				foreach (var str in Operands.Select(x => x.ToString()).Interspace(", ")) builder.Append(str);
				builder.Append(")");
			}

			return builder.ToString();
		}

		public Int32 GetSize()
		{
			return 1 + Operands.Sum(x => x.GetSize());
		}

		public Int32 Write(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var startingoffset = offset;

			buffer[offset++] = Definition.OpCode;
			foreach (var operand in Operands) offset += operand.Write(buffer, offset);

			return offset - startingoffset;
		}

		public InstructionDefinition Definition { get; }

		public List<Operand> Operands { get; }
	}
}