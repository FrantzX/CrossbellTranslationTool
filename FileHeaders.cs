using System;
using System.Runtime.InteropServices;

namespace CrossbellTranslationTool.FileHeaders
{
	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_HEADER
	{
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
		public Byte[] MapName;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
		public Byte[] Location;

		public UInt16 MapIndex;
		public UInt16 MapDefaultBGM;
		public UInt32 Flags;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 6)]
		public UInt32[] IncludedScenario;

		public UInt32 StringTableOffset;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 5)]
		public UInt16[] ScnInfoOffset;

		public UInt16 FunctionTableOffset;
		public UInt16 FunctionTableSize;
		public UInt16 ChipFrameInfoOffset;
		public UInt16 PlaceNameOffset;
		public Byte PlaceNameNumber;
		public Byte PreInitFunctionIndex;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 5)]
		public Byte[] ScnInfoNumber;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 3)]
		public Byte[] Unknown_51;

		public SCENARIO_INFORMATION Information;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_INFORMATION
	{
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 60)]
		public Byte[] Dummy;

		public Byte InitScenaIndex;
		public Byte InitFunctionIndex;
		public Byte EntryScenaIndex;
		public Byte EntryFunctionIndex;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_CHIP
	{
		public UInt32 FileIndex;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_MONSTER
	{
		public Int32 X;
		public Int32 Y;
		public Int32 Z;
		public UInt32 Unknown_0C;
		public UInt16 BattleInfoOffset;
		public UInt16 Unknown_12;
		public UInt16 ChipIndex;
		public UInt16 Unknown_16;
		public UInt32 StandFrameInfoIndex;
		public UInt32 MoveFrameInfoIndex;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_NPC
	{
		public Int32 X;
		public Int32 Y;
		public Int32 Z;
		public UInt16 Unknown1;
		public UInt16 Unknown2;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
		public Byte[] Unknown3;

		public Byte InitScenaIndex;
		public Byte InitFunctionIndex;
		public Byte TalkScenaIndex;
		public Byte TalkFunctionIndex;
		public UInt16 Unknown4;
		public UInt16 Unknown5;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_EVENT
	{
		public Single X;
		public Single Y;
		public Single Z;
		public Single Range;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 16)]
		public Single[] UnknownFloat_10;

		public UInt16 Flags;
		public UInt16 ScenaIndex;
		public UInt16 FunctionIndex;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
		public Byte[] Reserve;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_ATBONUS
	{
		public Byte Nothing;
		public Byte HpHeal10;
		public Byte HpHeal50;
		public Byte EpHeal10;
		public Byte EpHeal50;
		public Byte CpHeal10;
		public Byte CpHeal50;
		public Byte Sepith;
		public Byte Critical;
		public Byte Vanish;
		public Byte Death;
		public Byte Guard;
		public Byte Rush;
		public Byte ArtsGuard;
		public Byte TeamRush;
		public Byte Unknown;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_ACTOR
	{
		public Int32 TriggerX;
		public Int32 TriggerZ;
		public Int32 TriggerY;
		public UInt32 TriggerRange;
		public Int32 ActorX;
		public Int32 ActorZ;
		public Int32 ActorY;
		public UInt16 Flags;
		public UInt16 TalkScenaIndex;
		public UInt16 TalkFunctionIndex;
		public UInt16 Unknown_22;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_PLACENAME
	{
		public Single X;
		public Single Y;
		public Single Z;
		public UInt16 Flags1;
		public UInt16 Flags2;
		public UInt32 NameOffset;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_CHIPFRAMEINFO
	{
		public static SCENARIO_CHIPFRAMEINFO Read(FileReader reader)
		{
			var obj = new SCENARIO_CHIPFRAMEINFO();

			obj.Speed = reader.ReadUInt16();
			obj.Reserve = reader.ReadByte();
			obj.SubChipCount = reader.ReadByte();
			obj.SubChipIndex = reader.ReadBytes(obj.SubChipCount);

			var remander = obj.SubChipCount % 8;
			if (remander != 0) reader.Position += (8 - remander);

			return obj;
		}

		public static Byte[] Write(SCENARIO_CHIPFRAMEINFO input)
		{
			var buffer = new Byte[4 + MathUtil.RoundUp(input.SubChipCount, 8)];

			BinaryIO.WriteIntoBuffer(buffer, 0, input.Speed, Endian.LittleEndian);

			buffer[2] = input.Reserve;
			buffer[3] = input.SubChipCount;

			for (var i = 0; i != input.SubChipCount; ++i) buffer[4 + i] = input.SubChipIndex[i];

			return buffer;
		}

		public UInt16 Speed;
		public Byte Reserve;
		public Byte SubChipCount;

		[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)]
		public Byte[] SubChipIndex;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_BATTLE
	{
		public UInt16 Flags;
		public UInt16 Level;
		public Byte Unknown_04;
		public Byte Vision;
		public Byte MoveRange;
		public Byte CanMove;
		public UInt16 MoveSpeed;
		public UInt16 Unknown_0A;
		public UInt32 BattleMapOffset;
		public UInt32 SepithOffset;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
		public Byte[] Probability;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_BATTLEMONSTERPOSITION
	{
		public Byte X;
		public Byte Y;
		public UInt16 Rotation;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_BATTLEMONSTERINFO
	{
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 8)]
		public UInt32[] MsFileIndex;

		public UInt16 PositionNormalOffset;
		public UInt16 PositionEnemyAdvantageOffset;
		public UInt16 BgmNormal;
		public UInt16 BgmDanger;
		public UInt32 ATBonusOffset;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct SCENARIO_BATTLESEPITH
	{
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 7)]
		public Byte[] Values;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct MONSTER_HEADER
	{
		public UInt32 ASFile;
		public UInt16 Level;
		public UInt32 MaximumHP;
		public UInt32 InitialHP;
		public UInt16 MaximumEP;
		public UInt16 InitialEP;
		public UInt16 MaximumCP;
		public UInt16 InitialCP;

		public UInt16 SPD;
		public UInt16 MoveSPD;
		public UInt16 MOV;
		public UInt16 STR;
		public UInt16 DEF;
		public UInt16 ATS;
		public UInt16 ADF;
		public UInt16 DEX;
		public UInt16 AGL;
		public UInt16 RNG;

		public UInt16 Unknown_2A;
		public UInt16 EXP;
		public UInt16 Unknown_2E;
		public Byte Unknown_30;
		public UInt16 AIType;
		public UInt16 Unknown_33;
		public Byte Unknown_35;
		public UInt16 Unknown_36;
		public UInt16 EnemyFlags;
		public UInt16 BattleFlags;
		public UInt16 Unknown_3C;
		public UInt16 Unknown_3E;
		public Byte Sex;
		public Byte Unknown_41;
		public UInt32 CharSize;
		public UInt32 DefaultEffectX;
		public UInt32 DefaultEffectZ;
		public UInt32 DefaultEffectY;
		public Byte Unknown_52;
		public Byte Unknown_53;
		public Byte Unknown_54;
		public Byte Unknown_55;

		public UInt32 Symbol;
		public UInt32 Resistance;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 7)]
		public UInt16[] AttributeRate;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 7)]
		public Byte[] Sepith;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 2)]
		public UInt16[] DropItem;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
		public Byte[] DropRate;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 5)]
		public UInt16[] Equipment;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 4)]
		public UInt16[] Orbment;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct MONSTER_CRAFTAIINFO
	{
		public Byte Condition;
		public Byte Probability;
		public Byte Target;
		public Byte TargetCondition;
		public Byte AriaActionIndex;
		public Byte ActionIndex;
		public UInt16 CraftIndex;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
		public UInt32[] Parameter;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct MONSTER_CRAFTINFO
	{
		public UInt16 ActionIndex;
		public Byte Target;
		public Byte Unknown_3;
		public Byte Attribute;
		public Byte RangeType;
		public Byte State1;
		public Byte State2;
		public Byte RNG;
		public Byte RangeSize;
		public Byte AriaTime;
		public Byte SkillTime;
		public UInt16 EP_CP;
		public UInt16 RangeSize2;
		public UInt16 State1Parameter;
		public UInt16 State1Time;
		public UInt16 State2Parameter;
		public UInt16 State2Time;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct MONSTER_RUNAWAY
	{
		public Byte Type;
		public Byte Rate;
		public Byte Parameter;

	}
}
