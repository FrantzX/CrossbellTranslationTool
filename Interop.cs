using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CrossbellTranslationTool
{
	static class Interop
	{
		public static void FillBufferWithStruct<T>(ref T obj, Byte[] buffer, Int32 offset) where T : struct
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Marshal.StructureToPtr(obj, gchandle.AddrOfPinnedObject() + offset, false);

			gchandle.Free();
		}

		public static Byte[] GetBytesOfStruct<T>(ref T obj) where T : struct
		{
			var size = Marshal.SizeOf<T>();

			var buffer = new Byte[size];

			FillBufferWithStruct(ref obj, buffer, 0);

			return buffer;
		}

		public static T CreateStructFromBuffer<T>(Byte[] buffer, Int32 offset) where T : struct
		{
			Assert.IsNotNull(buffer, nameof(buffer));

			var gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			T obj = Marshal.PtrToStructure<T>(gchandle.AddrOfPinnedObject() + offset);

			gchandle.Free();

			return obj;
		}

		public static T ReadStructFromStream<T>(Stream stream) where T : struct
		{
			Assert.IsNotNull(stream, nameof(stream));

			var size = Marshal.SizeOf<T>();

			var buffer = new Byte[size];

			var bytesread = 0;
			while (bytesread < size) bytesread += stream.Read(buffer, bytesread, size - bytesread);

			return CreateStructFromBuffer<T>(buffer, 0);
		}

		public static IntPtr CopyStructToUnmanagedMemory<T>(ref T obj) where T : struct
		{
			var size = Marshal.SizeOf<T>();

			var memory = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(obj, memory, false);

			return memory;
		}
	}
}
