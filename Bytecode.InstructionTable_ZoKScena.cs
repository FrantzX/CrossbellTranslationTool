using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossbellTranslationTool.Bytecode
{
	static class InstructionTalble_ZoKScena
	{
		static InstructionTalble_ZoKScena()
		{
			ExitThread = new InstructionDefinition(0x00, "ExitThread", InstructionFlags.None, DefaultBuildInstruction);
			Return = new InstructionDefinition(0x01, "Return", InstructionFlags.EndBlock, DefaultBuildInstruction);
			Jc = new InstructionDefinition(0x02, "Jc", InstructionFlags.StartBlock, DefaultBuildInstruction, OperandType.Expression, OperandType.InstructionOffset);
			Jump = new InstructionDefinition(0x03, "Jump", InstructionFlags.Jump, DefaultBuildInstruction, OperandType.InstructionOffset);
			Switch = new InstructionDefinition(0x04, "Switch", InstructionFlags.EndBlock, BuildInstruction_Switch);
			Call = new InstructionDefinition(0x05, "Call", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			NewScene = new InstructionDefinition(0x06, "NewScene", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.Byte, OperandType.Byte, OperandType.Byte);
			IdleLoop = new InstructionDefinition(0x07, "IdleLoop", InstructionFlags.None, DefaultBuildInstruction);
			Sleep = new InstructionDefinition(0x08, "Sleep", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SetMapFlags = new InstructionDefinition(0x09, "SetMapFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32);
			ClearMapFlags = new InstructionDefinition(0x0A, "ClearMapFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32);
			FadeToDark = new InstructionDefinition(0x0B, "FadeToDark", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32, OperandType.Int32, OperandType.SByte);
			FadeToBright = new InstructionDefinition(0x0C, "FadeToBright", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32, OperandType.Int32);
			OP_0D = new InstructionDefinition(0x0D, "OP_0D", InstructionFlags.None, DefaultBuildInstruction);
			Fade = new InstructionDefinition(0x0E, "Fade", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32);
			Battle = new InstructionDefinition(0x0F, "Battle", InstructionFlags.None, BuildInstruction_Battle);
			OP_10 = new InstructionDefinition(0x10, "OP_10", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_11 = new InstructionDefinition(0x11, "OP_11", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			StopSound = new InstructionDefinition(0x12, "StopSound", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.Byte);
			OP_13 = new InstructionDefinition(0x13, "OP_13", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			BlurSwitch = new InstructionDefinition(0x14, "BlurSwitch", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.Byte, OperandType.UInt32);
			CancelBlur = new InstructionDefinition(0x15, "CancelBlur", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32);
			OP_16 = new InstructionDefinition(0x16, "OP_16", InstructionFlags.None, BuildInstruction_0x16);
			ShowSaveMenu = new InstructionDefinition(0x17, "ShowSaveMenu", InstructionFlags.None, DefaultBuildInstruction);
			EventBegin = new InstructionDefinition(0x19, "EventBegin", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			EventEnd = new InstructionDefinition(0x1A, "EventEnd", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_1B = new InstructionDefinition(0x1B, "OP_1B", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16);
			OP_1C = new InstructionDefinition(0x1C, "OP_1C", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt16, OperandType.UInt16);
			SetBarrier = new InstructionDefinition(0x1D, "SetBarrier", InstructionFlags.None, BuildInstruction_SetBarrier);
			PlayBGM = new InstructionDefinition(0x1E, "PlayBGM", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Byte);
			OP_1F = new InstructionDefinition(0x1F, "OP_1F", InstructionFlags.None, DefaultBuildInstruction);
			VolumeBGM = new InstructionDefinition(0x20, "VolumeBGM", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32);
			StopBGM = new InstructionDefinition(0x21, "StopBGM", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32);
			WaitBGM = new InstructionDefinition(0x22, "WaitBGM", InstructionFlags.None, DefaultBuildInstruction);
			Sound = new InstructionDefinition(0x23, "Sound", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Byte, OperandType.Byte);
			OP_24 = new InstructionDefinition(0x24, "OP_24", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_25 = new InstructionDefinition(0x25, "OP_25", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			SoundDistance = new InstructionDefinition(0x26, "SoundDistance", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.Byte, OperandType.UInt32);
			SoundLoad = new InstructionDefinition(0x27, "SoundLoad", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			Yield = new InstructionDefinition(0x28, "Yield", InstructionFlags.None, DefaultBuildInstruction);
			OP_29 = new InstructionDefinition(0x29, "OP_29", InstructionFlags.None, BuildInstruction_0x29);
			OP_2A = new InstructionDefinition(0x2A, "OP_2A", InstructionFlags.None, BuildInstruction_0x2A);
			OP_2B = new InstructionDefinition(0x2B, "OP_2B", InstructionFlags.None, BuildInstruction_0x2B);
			OP_2C = new InstructionDefinition(0x2C, "OP_2C", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			OP_2D = new InstructionDefinition(0x2D, "OP_2D", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			AddParty = new InstructionDefinition(0x2E, "AddParty", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte);
			RemoveParty = new InstructionDefinition(0x2F, "RemoveParty", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			ClearParty = new InstructionDefinition(0x30, "ClearParty", InstructionFlags.None, DefaultBuildInstruction);
			OP_31 = new InstructionDefinition(0x31, "OP_31", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_32 = new InstructionDefinition(0x32, "OP_32", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16);
			RemoveCraft = new InstructionDefinition(0x35, "RemoveCraft", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			AddCraft = new InstructionDefinition(0x36, "AddCraft", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_37 = new InstructionDefinition(0x37, "OP_37", InstructionFlags.None, DefaultBuildInstruction);
			OP_38 = new InstructionDefinition(0x38, "OP_38", InstructionFlags.None, BuildInstruction_0x38);
			AddSepith = new InstructionDefinition(0x39, "AddSepith", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			SubSepith = new InstructionDefinition(0x3A, "SubSepith", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			AddMira = new InstructionDefinition(0x3B, "AddMira", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SubMira = new InstructionDefinition(0x3C, "SubMira", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_3D = new InstructionDefinition(0x3D, "OP_3D", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_3E = new InstructionDefinition(0x3E, "OP_3E", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			AddItemNumber = new InstructionDefinition(0x3F, "AddItemNumber", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Int16);
			SubItemNumber = new InstructionDefinition(0x40, "SubItemNumber", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Int16);
			GetItemNumber = new InstructionDefinition(0x41, "GetItemNumber", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_42 = new InstructionDefinition(0x42, "OP_42", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16, OperandType.Byte);
			GetPartyIndex = new InstructionDefinition(0x43, "GetPartyIndex", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			BeginChrThread = new InstructionDefinition(0x44, "BeginChrThread", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Byte, OperandType.Byte);
			EndChrThread = new InstructionDefinition(0x45, "EndChrThread", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			QueueWorkItem = new InstructionDefinition(0x46, "QueueWorkItem", InstructionFlags.None, BuildInstruction_QueueWorkItem);
			QueueWorkItem2 = new InstructionDefinition(0x47, "QueueWorkItem2", InstructionFlags.None, BuildInstruction_QueueWorkItem2);
			WaitChrThread = new InstructionDefinition(0x48, "WaitChrThread", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_49 = new InstructionDefinition(0x49, "OP_49", InstructionFlags.None, DefaultBuildInstruction);
			Event = new InstructionDefinition(0x4A, "Event", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_4B = new InstructionDefinition(0x4B, "OP_4B", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_4C = new InstructionDefinition(0x4C, "OP_4C", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_4D = new InstructionDefinition(0x4D, "OP_4D", InstructionFlags.None, DefaultBuildInstruction);
			RunExpression = new InstructionDefinition(0x4E, "RunExpression", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Expression);
			OP_4F = new InstructionDefinition(0x4F, "OP_4F", InstructionFlags.None, DefaultBuildInstruction);
			OP_50 = new InstructionDefinition(0x50, "GetValueIndex", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Expression);
			OP_51 = new InstructionDefinition(0x51, "OP_51", InstructionFlags.None, DefaultBuildInstruction);
			OP_52 = new InstructionDefinition(0x52, "OP_52", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Expression);
			TalkBegin = new InstructionDefinition(0x53, "TalkBegin", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			TalkEnd = new InstructionDefinition(0x54, "TalkEnd", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			AnonymousTalk = new InstructionDefinition(0x55, "AnonymousTalk", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.String);
			OP_56 = new InstructionDefinition(0x56, "OP_56", InstructionFlags.None, DefaultBuildInstruction);
			OP_57 = new InstructionDefinition(0x57, "OP_57", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			MenuTitle = new InstructionDefinition(0x58, "MenuTitle", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16, OperandType.Int16, OperandType.String);
			CloseMessageWindow = new InstructionDefinition(0x59, "CloseMessageWindow", InstructionFlags.None, DefaultBuildInstruction);
			OP_5A = new InstructionDefinition(0x5A, "OP_5A", InstructionFlags.None, DefaultBuildInstruction);
			SetMessageWindowPos = new InstructionDefinition(0x5B, "SetMessageWindowPos", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16, OperandType.Int16, OperandType.Int16);
			ChrTalk = new InstructionDefinition(0x5C, "ChrTalk", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.String);
			NpcTalk = new InstructionDefinition(0x5D, "NpcTalk", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.String, OperandType.String);
			Menu = new InstructionDefinition(0x5E, "Menu", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16, OperandType.Int16, OperandType.SByte, OperandType.String);
			MenuEnd = new InstructionDefinition(0x5F, "MenuEnd", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_60 = new InstructionDefinition(0x60, "OP_60", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SetChrName = new InstructionDefinition(0x61, "SetChrName", InstructionFlags.None, DefaultBuildInstruction, OperandType.String);
			OP_62 = new InstructionDefinition(0x62, "OP_62", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_63 = new InstructionDefinition(0x63, "OP_63", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.Byte, OperandType.Byte, OperandType.UInt32, OperandType.Byte);
			OP_64 = new InstructionDefinition(0x64, "OP_64", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_65 = new InstructionDefinition(0x65, "OP_65", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_66 = new InstructionDefinition(0x66, "OP_66", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_67 = new InstructionDefinition(0x67, "OP_67", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_68 = new InstructionDefinition(0x68, "OP_68", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32);
			OP_69 = new InstructionDefinition(0x69, "OP_69", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_6A = new InstructionDefinition(0x6A, "OP_6A", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32);
			OP_6B = new InstructionDefinition(0x6B, "OP_6B", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SetCameraDistance = new InstructionDefinition(0x6C, "SetCameraDistance", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32, OperandType.Int32);
			MoveCamera = new InstructionDefinition(0x6D, "MoveCamera", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16, OperandType.Int16, OperandType.Int32);
			OP_6E = new InstructionDefinition(0x6E, "OP_6E", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32, OperandType.Int32);
			OP_6F = new InstructionDefinition(0x6F, "OP_6F", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_70 = new InstructionDefinition(0x70, "OP_70", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_71 = new InstructionDefinition(0x71, "OP_71", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32);
			SetMapObjFlags = new InstructionDefinition(0x72, "SetMapObjFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32);
			ClearMapObjFlags = new InstructionDefinition(0x73, "ClearMapObjFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32);
			OP_74 = new InstructionDefinition(0x74, "OP_74", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_75 = new InstructionDefinition(0x75, "OP_75", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			SetMapObjFrame = new InstructionDefinition(0x76, "SetMapObjFrame", InstructionFlags.None, BuildInstruction_SetMapObjFrame);
			OP_77 = new InstructionDefinition(0x77, "OP_77", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_78 = new InstructionDefinition(0x78, "OP_78", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_79 = new InstructionDefinition(0x79, "OP_79", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SetEventSkip = new InstructionDefinition(0x7A, "SetEventSkip", InstructionFlags.StartBlock, DefaultBuildInstruction, OperandType.Byte, OperandType.InstructionOffset);
			OP_7B = new InstructionDefinition(0x7B, "OP_7B", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_7D = new InstructionDefinition(0x7D, "OP_7D", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			OP_82 = new InstructionDefinition(0x82, "OP_82", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			SetChrChip = new InstructionDefinition(0x83, "SetChrChip", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16);
			OP_84 = new InstructionDefinition(0x84, "OP_84", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			LoadEffect = new InstructionDefinition(0x85, "LoadEffect", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.String);
			PlayEffect = new InstructionDefinition(0x86, "PlayEffect", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int16, OperandType.Int16, OperandType.Int16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32);
			OP_87 = new InstructionDefinition(0x87, "OP_87", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.String, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			StopEffect = new InstructionDefinition(0x88, "StopEffect", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_89 = new InstructionDefinition(0x89, "OP_89", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_8A = new InstructionDefinition(0x8A, "OP_8A", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_8B = new InstructionDefinition(0x8B, "OP_8B", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			SetChrChipByIndex = new InstructionDefinition(0x8C, "SetChrChipByIndex", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			SetChrSubChip = new InstructionDefinition(0x8D, "SetChrSubChip", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_8E = new InstructionDefinition(0x8E, "OP_8E", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.String);
			SetChrPos = new InstructionDefinition(0x8F, "SetChrPos", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.UInt16);
			OP_90 = new InstructionDefinition(0x90, "OP_90", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int16);
			TurnDirection = new InstructionDefinition(0x91, "TurnDirection", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16);
			OP_92 = new InstructionDefinition(0x92, "OP_92", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt16);
			OP_93 = new InstructionDefinition(0x93, "OP_93", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16);
			OP_94 = new InstructionDefinition(0x94, "OP_94", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_95 = new InstructionDefinition(0x95, "OP_95", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Byte);
			OP_96 = new InstructionDefinition(0x96, "OP_96", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_97 = new InstructionDefinition(0x97, "OP_97", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_98 = new InstructionDefinition(0x98, "OP_98", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_99 = new InstructionDefinition(0x99, "OP_99", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_9A = new InstructionDefinition(0x9A, "OP_9A", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_9B = new InstructionDefinition(0x9B, "OP_9B", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.Byte);
			OP_9C = new InstructionDefinition(0x9C, "OP_9C", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_9D = new InstructionDefinition(0x9D, "OP_9D", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_9E = new InstructionDefinition(0x9E, "OP_9E", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt16);
			OP_9F = new InstructionDefinition(0x9F, "OP_9F", InstructionFlags.None, BuildInstruction_0x9F);
			OP_A0 = new InstructionDefinition(0xA0, "OP_A0", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16, OperandType.Byte, OperandType.Byte);
			OP_A1 = new InstructionDefinition(0xA1, "OP_A1", InstructionFlags.None, BuildInstruction_0xA1);
			SetChrFlags = new InstructionDefinition(0xA2, "SetChrFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			ClearChrFlags = new InstructionDefinition(0xA3, "ClearChrFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			SetChrBattleFlags = new InstructionDefinition(0xA4, "SetChrBattleFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			ClearChrBattleFlags = new InstructionDefinition(0xA5, "ClearChrBattleFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			OP_A6 = new InstructionDefinition(0xA6, "OP_A6", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_A7 = new InstructionDefinition(0xA7, "OP_A7", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			OP_A8 = new InstructionDefinition(0xA8, "OP_A8", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			SetScenarioFlags = new InstructionDefinition(0xA9, "SetScenarioFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			ClearScenarioFlags = new InstructionDefinition(0xAA, "ClearScenarioFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_AB = new InstructionDefinition(0xAB, "OP_AB", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_AC = new InstructionDefinition(0xAC, "OP_AC", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_AD = new InstructionDefinition(0xAD, "OP_AD", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_AE = new InstructionDefinition(0xAE, "OP_AE", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			OP_AF = new InstructionDefinition(0xAF, "OP_AF", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_B0 = new InstructionDefinition(0xB0, "OP_B0", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OutputDebugInt = new InstructionDefinition(0xB1, "OutputDebugInt", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_B2 = new InstructionDefinition(0xB2, "OP_B2", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_B3 = new InstructionDefinition(0xB3, "OP_B3", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			LoadOps = new InstructionDefinition(0xB4, "LoadOps", InstructionFlags.None, DefaultBuildInstruction);
			ModifyEventFlags = new InstructionDefinition(0xB5, "ModifyEventFlags", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16);
			PlayMovie = new InstructionDefinition(0xB6, "PlayMovie", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.String, OperandType.UInt16, OperandType.UInt16);
			OP_B7 = new InstructionDefinition(0xB7, "OP_B7", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			ReplaceBGM = new InstructionDefinition(0xB8, "ReplaceBGM", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16);
			OP_BA = new InstructionDefinition(0xBA, "OP_BA", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			UseItem = new InstructionDefinition(0xBB, "UseItem", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			OP_BC = new InstructionDefinition(0xBC, "OP_BC", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_BD = new InstructionDefinition(0xBD, "OP_BD", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			SetChrChipPat = new InstructionDefinition(0xBE, "SetChrChipPat", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			LoadChrChipPat = new InstructionDefinition(0xC0, "LoadChrChipPat", InstructionFlags.None, DefaultBuildInstruction);
			OP_C1 = new InstructionDefinition(0xC1, "OP_C1", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.Byte, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32, OperandType.Int32);
			OP_C2 = new InstructionDefinition(0xC2, "OP_C2", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt16, OperandType.UInt16);
			MiniGame = new InstructionDefinition(0xC3, "MiniGame", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_C5 = new InstructionDefinition(0xC5, "OP_C5", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_C7 = new InstructionDefinition(0xC7, "OP_C7", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32);
			CreatePortrait = new InstructionDefinition(0xC8, "CreatePortrait", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt16, OperandType.UInt32, OperandType.Byte, OperandType.String);
			OP_C9 = new InstructionDefinition(0xC9, "OP_C9", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_CA = new InstructionDefinition(0xCA, "OP_CA", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte);
			PlaceName2 = new InstructionDefinition(0xCB, "PlaceName2", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int16, OperandType.Int16, OperandType.String, OperandType.Byte, OperandType.Int16);
			PartySelect = new InstructionDefinition(0xCC, "PartySelect", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_CD = new InstructionDefinition(0xCD, "OP_CD", InstructionFlags.None, BuildInstruction_0xCD);
			MenuCmd = new InstructionDefinition(0xCE, "MenuCmd", InstructionFlags.None, BuildInstruction_MenuCmd);
			OP_CF = new InstructionDefinition(0xCF, "OP_CF", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_D0 = new InstructionDefinition(0xD0, "OP_D0", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Expression);
			OP_D1 = new InstructionDefinition(0xD1, "OP_D1", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.String);
			OP_D2 = new InstructionDefinition(0xD2, "OP_D2", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.UInt32);
			OP_D3 = new InstructionDefinition(0xD3, "OP_D3", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			LoadChrToIndex = new InstructionDefinition(0xD4, "LoadChrToIndex", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.Byte);
			OP_D5 = new InstructionDefinition(0xD5, "OP_D5", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_D6 = new InstructionDefinition(0xD6, "OP_D6", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_D7 = new InstructionDefinition(0xD7, "OP_D7", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_D8 = new InstructionDefinition(0xD8, "OP_D8", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_DA = new InstructionDefinition(0xDA, "OP_DA", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_DB = new InstructionDefinition(0xDB, "OP_DB", InstructionFlags.None, DefaultBuildInstruction);
			OP_DC = new InstructionDefinition(0xDC, "OP_DC", InstructionFlags.None, DefaultBuildInstruction, OperandType.String);
			LoadAnimeChip = new InstructionDefinition(0xDD, "LoadAnimeChip", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte, OperandType.Byte);
			OP_DE = new InstructionDefinition(0xDE, "OP_DE", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte);
			OP_E0 = new InstructionDefinition(0xE0, "OP_E0", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_E1 = new InstructionDefinition(0xE1, "OP_E1", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
			OP_E2 = new InstructionDefinition(0xE2, "OP_E2", InstructionFlags.None, BuildInstruction_0xE2);
			OP_E3 = new InstructionDefinition(0xE3, "OP_E3", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_E4 = new InstructionDefinition(0xE4, "OP_E4", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.Byte, OperandType.UInt32);
			OP_E5 = new InstructionDefinition(0xE5, "OP_E5", InstructionFlags.None, DefaultBuildInstruction);
			OP_E6 = new InstructionDefinition(0xE6, "OP_E6", InstructionFlags.None, DefaultBuildInstruction);
			ShowSaveClearMenu = new InstructionDefinition(0xE7, "ShowSaveClearMenu", InstructionFlags.None, DefaultBuildInstruction);
			OP_EE = new InstructionDefinition(0xEE, "OP_EE", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt16);
			OP_F1 = new InstructionDefinition(0xF1, "OP_F1", InstructionFlags.None, DefaultBuildInstruction, OperandType.Int32);
			OP_F2 = new InstructionDefinition(0xF2, "OP_F2", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_F8 = new InstructionDefinition(0xF8, "OP_F8", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_F9 = new InstructionDefinition(0xF9, "OP_F9", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.Byte);
			OP_FA = new InstructionDefinition(0xFA, "OP_FA", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16);
			OP_FB = new InstructionDefinition(0xFB, "OP_FB", InstructionFlags.None, DefaultBuildInstruction, OperandType.UInt16, OperandType.UInt16);
			OP_FC = new InstructionDefinition(0xFC, "OP_FC", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte);
			OP_FD = new InstructionDefinition(0xFD, "OP_FD", InstructionFlags.None, DefaultBuildInstruction, OperandType.Byte, OperandType.UInt32, OperandType.UInt32, OperandType.UInt32);
		}

		static void DefaultBuildInstruction(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = state.ReadOperands(state.Instruction.Definition.DefaultOperandTypes);

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_Switch(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var expression = state.ReadOperand(OperandType.Expression);
			state.Instruction.Operands.Add(expression);

			var optioncount = state.ReadOperand(OperandType.Byte);
			state.Instruction.Operands.Add(optioncount);

			for (var i = 0; i != optioncount.GetValue<Byte>(); ++i)
			{
				var optionid = state.ReadOperand(OperandType.UInt16);
				var optionoffset = state.ReadOperand(OperandType.InstructionOffset);

				state.Instruction.Operands.Add(optionid);
				state.Instruction.Operands.Add(optionoffset);
			}

			var defaultoffset = state.ReadOperand(OperandType.InstructionOffset);
			state.Instruction.Operands.Add(defaultoffset);
		}

		static void BuildInstruction_Battle(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.BattleOffset));
			operands.Add(state.ReadOperand(OperandType.UInt32));

			if (operands[0].GetValue<UInt32>() != 0xFFFFFFFF)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.UInt16));
			}
			else
			{
				operands.Add(state.ReadOperand(OperandType.String));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.UInt16));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_SetBarrier(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[0].GetValue<Byte>();

			if (key == 0)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
			}

			if (key == 2 || key == 3)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x16(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[0].GetValue<Byte>();

			if (key == 2)
			{
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt32));
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.UInt32));
			}

			if (key == 3)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x29(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[1].GetValue<Byte>();

			if (key == 1 || key == 2)
			{
				operands.Add(state.ReadOperand(OperandType.UInt16));
			}

			if (key == 3 || key == 4)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x2A(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[1].GetValue<Byte>();

			if (key == 1)
			{
				operands.Add(state.ReadOperand(OperandType.UInt16));
			}
			else
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x2B(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			for (var i = 0; i != 12; ++i)
			{
				var operand = state.ReadOperand(OperandType.UInt16);
				operands.Add(operand);

				if (operand.GetValue<UInt16>() == 0xFFFF) break;
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x38(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[1].GetValue<Byte>();

			if (key == 0x7F || (key >= 0x80 && key <= 0x87))
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_QueueWorkItem(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var blocklength = 1 + operands[2].GetValue<Byte>();

			var instructionblock = state.ReadInstructionBlock((UInt32)blocklength);

			operands.AddRange(instructionblock.Select(x => new Operand(OperandType.Instruction, x.Instruction)));

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_QueueWorkItem2(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var blocklength = 6 + operands[2].GetValue<Byte>();

			var instructionblock = state.ReadInstructionBlock((UInt32)blocklength);

			operands.AddRange(instructionblock.Select(x => new Operand(OperandType.Instruction, x.Instruction)));

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_SetMapObjFrame(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.String));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[2].GetValue<Byte>();

			if (key == 0 || key == 1 || key == 3)
			{
				operands.Add(state.ReadOperand(OperandType.UInt32));
			}

			if (key == 2)
			{
				operands.Add(state.ReadOperand(OperandType.String));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0x9F(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[0].GetValue<Byte>();

			if (key == 0)
			{
				operands.Add(state.ReadOperand(OperandType.UInt16));
			}
			else if (key == 1)
			{
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Int32));
			}
			else
			{
				operands.Add(state.ReadOperand(OperandType.UInt16));
				operands.Add(state.ReadOperand(OperandType.Int32));
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0xA1(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.UInt16));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var count = operands[2].GetValue<Byte>();

			for (var i = 0; i != count; ++i) operands.Add(state.ReadOperand(OperandType.Byte));

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0xCD(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[0].GetValue<Byte>();

			if (key != 0)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_MenuCmd(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));
			operands.Add(state.ReadOperand(OperandType.Byte));

			var menutype = operands[0].GetValue<Byte>();

			if (menutype == 1)
			{
				operands.Add(state.ReadOperand(OperandType.String));
			}
			else if (menutype == 2)
			{
				operands.Add(state.ReadOperand(OperandType.Int16));
				operands.Add(state.ReadOperand(OperandType.Int16));
				operands.Add(state.ReadOperand(OperandType.Byte));
			}
			else if (menutype == 3 || menutype == 4 || menutype == 5)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		static void BuildInstruction_0xE2(DisassemblyState state)
		{
			Assert.IsNotNull(state, nameof(state));

			var operands = new List<Operand>();

			operands.Add(state.ReadOperand(OperandType.Byte));

			var key = operands[0].GetValue<Byte>();

			if (key == 0)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
				operands.Add(state.ReadOperand(OperandType.Byte));
			}
			else if (key == 1 || key == 2)
			{
				operands.Add(state.ReadOperand(OperandType.Byte));
			}

			state.Instruction.Operands.AddRange(operands);
		}

		public static InstructionDefinition ExitThread { get; }
		public static InstructionDefinition Return { get; }
		public static InstructionDefinition Jc { get; }
		public static InstructionDefinition Jump { get; }
		public static InstructionDefinition Switch { get; }
		public static InstructionDefinition Call { get; }
		public static InstructionDefinition NewScene { get; }
		public static InstructionDefinition IdleLoop { get; }
		public static InstructionDefinition Sleep { get; }
		public static InstructionDefinition SetMapFlags { get; }
		public static InstructionDefinition ClearMapFlags { get; }
		public static InstructionDefinition FadeToDark { get; }
		public static InstructionDefinition FadeToBright { get; }
		public static InstructionDefinition OP_0D { get; }
		public static InstructionDefinition Fade { get; }
		public static InstructionDefinition Battle { get; }
		public static InstructionDefinition OP_10 { get; }
		public static InstructionDefinition OP_11 { get; }
		public static InstructionDefinition StopSound { get; }
		public static InstructionDefinition OP_13 { get; }
		public static InstructionDefinition BlurSwitch { get; }
		public static InstructionDefinition CancelBlur { get; }
		public static InstructionDefinition OP_16 { get; }
		public static InstructionDefinition ShowSaveMenu { get; }
		public static InstructionDefinition EventBegin { get; }
		public static InstructionDefinition EventEnd { get; }
		public static InstructionDefinition OP_1B { get; }
		public static InstructionDefinition OP_1C { get; }
		public static InstructionDefinition SetBarrier { get; }
		public static InstructionDefinition PlayBGM { get; }
		public static InstructionDefinition OP_1F { get; }
		public static InstructionDefinition VolumeBGM { get; }
		public static InstructionDefinition StopBGM { get; }
		public static InstructionDefinition WaitBGM { get; }
		public static InstructionDefinition Sound { get; }
		public static InstructionDefinition OP_24 { get; }
		public static InstructionDefinition OP_25 { get; }
		public static InstructionDefinition SoundDistance { get; }
		public static InstructionDefinition SoundLoad { get; }
		public static InstructionDefinition Yield { get; }
		public static InstructionDefinition OP_29 { get; }
		public static InstructionDefinition OP_2A { get; }
		public static InstructionDefinition OP_2B { get; }
		public static InstructionDefinition OP_2C { get; }
		public static InstructionDefinition OP_2D { get; }
		public static InstructionDefinition AddParty { get; }
		public static InstructionDefinition RemoveParty { get; }
		public static InstructionDefinition ClearParty { get; }
		public static InstructionDefinition OP_31 { get; }
		public static InstructionDefinition OP_32 { get; }
		public static InstructionDefinition RemoveCraft { get; }
		public static InstructionDefinition AddCraft { get; }
		public static InstructionDefinition OP_37 { get; }
		public static InstructionDefinition OP_38 { get; }
		public static InstructionDefinition AddSepith { get; }
		public static InstructionDefinition SubSepith { get; }
		public static InstructionDefinition AddMira { get; }
		public static InstructionDefinition SubMira { get; }
		public static InstructionDefinition OP_3D { get; }
		public static InstructionDefinition OP_3E { get; }
		public static InstructionDefinition AddItemNumber { get; }
		public static InstructionDefinition SubItemNumber { get; }
		public static InstructionDefinition GetItemNumber { get; }
		public static InstructionDefinition OP_42 { get; }
		public static InstructionDefinition GetPartyIndex { get; }
		public static InstructionDefinition BeginChrThread { get; }
		public static InstructionDefinition EndChrThread { get; }
		public static InstructionDefinition QueueWorkItem { get; }
		public static InstructionDefinition QueueWorkItem2 { get; }
		public static InstructionDefinition WaitChrThread { get; }
		public static InstructionDefinition OP_49 { get; }
		public static InstructionDefinition Event { get; }
		public static InstructionDefinition OP_4B { get; }
		public static InstructionDefinition OP_4C { get; }
		public static InstructionDefinition OP_4D { get; }
		public static InstructionDefinition RunExpression { get; }
		public static InstructionDefinition OP_4F { get; }
		public static InstructionDefinition OP_50 { get; }
		public static InstructionDefinition OP_51 { get; }
		public static InstructionDefinition OP_52 { get; }
		public static InstructionDefinition TalkBegin { get; }
		public static InstructionDefinition TalkEnd { get; }
		public static InstructionDefinition AnonymousTalk { get; }
		public static InstructionDefinition OP_56 { get; }
		public static InstructionDefinition OP_57 { get; }
		public static InstructionDefinition MenuTitle { get; }
		public static InstructionDefinition CloseMessageWindow { get; }
		public static InstructionDefinition OP_5A { get; }
		public static InstructionDefinition SetMessageWindowPos { get; }
		public static InstructionDefinition ChrTalk { get; }
		public static InstructionDefinition NpcTalk { get; }
		public static InstructionDefinition Menu { get; }
		public static InstructionDefinition MenuEnd { get; }
		public static InstructionDefinition OP_60 { get; }
		public static InstructionDefinition SetChrName { get; }
		public static InstructionDefinition OP_62 { get; }
		public static InstructionDefinition OP_63 { get; }
		public static InstructionDefinition OP_64 { get; }
		public static InstructionDefinition OP_65 { get; }
		public static InstructionDefinition OP_66 { get; }
		public static InstructionDefinition OP_67 { get; }
		public static InstructionDefinition OP_68 { get; }
		public static InstructionDefinition OP_69 { get; }
		public static InstructionDefinition OP_6A { get; }
		public static InstructionDefinition OP_6B { get; }
		public static InstructionDefinition SetCameraDistance { get; }
		public static InstructionDefinition MoveCamera { get; }
		public static InstructionDefinition OP_6E { get; }
		public static InstructionDefinition OP_6F { get; }
		public static InstructionDefinition OP_70 { get; }
		public static InstructionDefinition OP_71 { get; }
		public static InstructionDefinition SetMapObjFlags { get; }
		public static InstructionDefinition ClearMapObjFlags { get; }
		public static InstructionDefinition OP_74 { get; }
		public static InstructionDefinition OP_75 { get; }
		public static InstructionDefinition SetMapObjFrame { get; }
		public static InstructionDefinition OP_77 { get; }
		public static InstructionDefinition OP_78 { get; }
		public static InstructionDefinition OP_79 { get; }
		public static InstructionDefinition SetEventSkip { get; }
		public static InstructionDefinition OP_7B { get; }
		public static InstructionDefinition OP_7D { get; }
		public static InstructionDefinition OP_82 { get; }
		public static InstructionDefinition SetChrChip { get; }
		public static InstructionDefinition OP_84 { get; }
		public static InstructionDefinition LoadEffect { get; }
		public static InstructionDefinition PlayEffect { get; }
		public static InstructionDefinition OP_87 { get; }
		public static InstructionDefinition StopEffect { get; }
		public static InstructionDefinition OP_89 { get; }
		public static InstructionDefinition OP_8A { get; }
		public static InstructionDefinition OP_8B { get; }
		public static InstructionDefinition SetChrChipByIndex { get; }
		public static InstructionDefinition SetChrSubChip { get; }
		public static InstructionDefinition OP_8E { get; }
		public static InstructionDefinition SetChrPos { get; }
		public static InstructionDefinition OP_90 { get; }
		public static InstructionDefinition TurnDirection { get; }
		public static InstructionDefinition OP_92 { get; }
		public static InstructionDefinition OP_93 { get; }
		public static InstructionDefinition OP_94 { get; }
		public static InstructionDefinition OP_95 { get; }
		public static InstructionDefinition OP_96 { get; }
		public static InstructionDefinition OP_97 { get; }
		public static InstructionDefinition OP_98 { get; }
		public static InstructionDefinition OP_99 { get; }
		public static InstructionDefinition OP_9A { get; }
		public static InstructionDefinition OP_9B { get; }
		public static InstructionDefinition OP_9C { get; }
		public static InstructionDefinition OP_9D { get; }
		public static InstructionDefinition OP_9E { get; }
		public static InstructionDefinition OP_9F { get; }
		public static InstructionDefinition OP_A0 { get; }
		public static InstructionDefinition OP_A1 { get; }
		public static InstructionDefinition SetChrFlags { get; }
		public static InstructionDefinition ClearChrFlags { get; }
		public static InstructionDefinition SetChrBattleFlags { get; }
		public static InstructionDefinition ClearChrBattleFlags { get; }
		public static InstructionDefinition OP_A6 { get; }
		public static InstructionDefinition OP_A7 { get; }
		public static InstructionDefinition OP_A8 { get; }
		public static InstructionDefinition SetScenarioFlags { get; }
		public static InstructionDefinition ClearScenarioFlags { get; }
		public static InstructionDefinition OP_AB { get; }
		public static InstructionDefinition OP_AC { get; }
		public static InstructionDefinition OP_AD { get; }
		public static InstructionDefinition OP_AE { get; }
		public static InstructionDefinition OP_AF { get; }
		public static InstructionDefinition OP_B0 { get; }
		public static InstructionDefinition OutputDebugInt { get; }
		public static InstructionDefinition OP_B2 { get; }
		public static InstructionDefinition OP_B3 { get; }
		public static InstructionDefinition LoadOps { get; }
		public static InstructionDefinition ModifyEventFlags { get; }
		public static InstructionDefinition PlayMovie { get; }
		public static InstructionDefinition OP_B7 { get; }
		public static InstructionDefinition ReplaceBGM { get; }
		public static InstructionDefinition OP_BA { get; }
		public static InstructionDefinition UseItem { get; }
		public static InstructionDefinition OP_BC { get; }
		public static InstructionDefinition OP_BD { get; }
		public static InstructionDefinition SetChrChipPat { get; }
		public static InstructionDefinition LoadChrChipPat { get; }
		public static InstructionDefinition OP_C1 { get; }
		public static InstructionDefinition OP_C2 { get; }
		public static InstructionDefinition MiniGame { get; }
		public static InstructionDefinition OP_C5 { get; }
		public static InstructionDefinition OP_C7 { get; }
		public static InstructionDefinition CreatePortrait { get; }
		public static InstructionDefinition OP_C9 { get; }
		public static InstructionDefinition OP_CA { get; }
		public static InstructionDefinition PlaceName2 { get; }
		public static InstructionDefinition PartySelect { get; }
		public static InstructionDefinition OP_CD { get; }
		public static InstructionDefinition MenuCmd { get; }
		public static InstructionDefinition OP_CF { get; }
		public static InstructionDefinition OP_D0 { get; }
		public static InstructionDefinition OP_D1 { get; }
		public static InstructionDefinition OP_D2 { get; }
		public static InstructionDefinition OP_D3 { get; }
		public static InstructionDefinition LoadChrToIndex { get; }
		public static InstructionDefinition OP_D5 { get; }
		public static InstructionDefinition OP_D6 { get; }
		public static InstructionDefinition OP_D7 { get; }
		public static InstructionDefinition OP_D8 { get; }
		public static InstructionDefinition OP_DA { get; }
		public static InstructionDefinition OP_DB { get; }
		public static InstructionDefinition OP_DC { get; }
		public static InstructionDefinition LoadAnimeChip { get; }
		public static InstructionDefinition OP_DE { get; }
		public static InstructionDefinition OP_E0 { get; }
		public static InstructionDefinition OP_E1 { get; }
		public static InstructionDefinition OP_E2 { get; }
		public static InstructionDefinition OP_E3 { get; }
		public static InstructionDefinition OP_E4 { get; }
		public static InstructionDefinition OP_E5 { get; }
		public static InstructionDefinition OP_E6 { get; }
		public static InstructionDefinition ShowSaveClearMenu { get; }
		public static InstructionDefinition OP_EE { get; }
		public static InstructionDefinition OP_F1 { get; }
		public static InstructionDefinition OP_F2 { get; }
		public static InstructionDefinition OP_F8 { get; }
		public static InstructionDefinition OP_F9 { get; }
		public static InstructionDefinition OP_FA { get; }
		public static InstructionDefinition OP_FB { get; }
		public static InstructionDefinition OP_FC { get; }
		public static InstructionDefinition OP_FD { get; }
	}
}
