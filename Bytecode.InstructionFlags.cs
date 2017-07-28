using System;

namespace CrossbellTranslationTool.Bytecode
{
	[Flags]
	enum InstructionFlags
	{
		None = 0,
		EndBlock = 1,
		StartBlock = 2,
		Call = 4 | StartBlock,
		Jump = 8 | EndBlock
	}
}