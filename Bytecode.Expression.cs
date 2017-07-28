using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Bytecode
{
	class Expression
	{
		public Expression()
		{
			Operations = new List<Operation>();
		}

		public override String ToString()
		{
			return "Expression";
		}

		public Int32 GetSize()
		{
			return Operations.Sum(x => x.GetSize());
		}

		public Int32 Write(Byte[] buffer, Int32 offset)
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var startingoffset = offset;

			foreach (var operation in Operations) offset += operation.Write(buffer, offset);

			return offset - startingoffset;
		}

		public List<Operation> Operations { get; }
	}
}