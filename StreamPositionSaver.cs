using System;
using System.IO;

namespace CrossbellTranslationTool
{
	class StreamPositionSaver : IDisposable
	{
		public StreamPositionSaver(Stream stream)
		{
			Assert.IsNotNull(stream, nameof(stream));

			Stream = stream;
			Position = stream.Position;
		}

		public void Dispose()
		{
			Stream.Position = Position;
		}

		Stream Stream { get; }

		Int64 Position { get; }
	}
}