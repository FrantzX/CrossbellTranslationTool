using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Bytecode
{
	class Operation
	{
		public Operation(OperationType type)
		{
			Assert.IsValidEnumeration(type, nameof(type), false);

			Type = type;
			Operands = new List<Operand>();
		}

		public override String ToString()
		{
			var builder = new System.Text.StringBuilder();

			builder.Append(Type);

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

			buffer[offset++] = (Byte)Type;
			foreach (var operand in Operands) offset += operand.Write(buffer, offset);

			return offset - startingoffset;
		}

		public OperationType Type { get; }

		public List<Operand> Operands { get; }
	}
}