using System;
using System.Collections.Generic;

namespace CrossbellTranslationTool.Bytecode
{
	class InstructionDefinition
	{
		public InstructionDefinition(Byte opcode, String name, InstructionFlags flags, Action<DisassemblyState> disassemblyfunction, params OperandType[] operandtypes)
		{
			Assert.IsValidString(name, nameof(name));
			Assert.IsValidEnumeration(flags, nameof(flags), false);
			Assert.IsNotNull(disassemblyfunction, nameof(disassemblyfunction));
			Assert.IsNotNull(operandtypes, nameof(operandtypes));

			OpCode = opcode;
			Name = name;
			Flags = flags;
			DisassemblyFunction = disassemblyfunction;
			DefaultOperandTypes = operandtypes.ToReadOnlyList();
		}

		public override String ToString()
		{
			return Name;
		}

		public Byte OpCode { get; }

		public String Name { get; }

		public InstructionFlags Flags { get; }

		public IList<OperandType> DefaultOperandTypes { get; }

		public Action<DisassemblyState> DisassemblyFunction { get; }
	}
}