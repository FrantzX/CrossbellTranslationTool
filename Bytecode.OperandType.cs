using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossbellTranslationTool.Bytecode
{
	enum OperandType
	{
		None,
		Byte,
		SByte,
		UInt16,
		Int16,
		UInt32,
		Int32,
		Instruction,
		Expression,
		Operation,
		String,
		InstructionOffset,
		BattleOffset
	}
}