using System;
using System.Collections.Generic;
using CrossbellTranslationTool.Iso9660;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrossbellTranslationTool.IO
{
	class IsoFileSystem : IFileSystem
	{
		public IsoFileSystem(IsoImage image, String rootpath)
		{
			Assert.IsNotNull(image, nameof(image));
			Assert.IsNotNull(rootpath, nameof(rootpath));

			Image = image;
			RootPath = rootpath;
		}

		public Boolean DoesFileExist(String filepath)
		{
			Assert.IsValidString(filepath, nameof(filepath));

			var totalfilepath = Path.Combine(RootPath, filepath);

			return Image.GetRecord(totalfilepath) != null;
		}

		public List<String> GetChildren(String directorypath, String searchpattern)
		{
			Assert.IsValidString(directorypath, nameof(directorypath));
			Assert.IsValidString(searchpattern, nameof(searchpattern));

			var totaldirectorypath = Path.Combine(RootPath, directorypath);
			var children = Image.GetChildren(totaldirectorypath);

			var regex = WildcardToRegex(searchpattern);

			return children.Where(x => Regex.IsMatch(x.FileIdentifier, regex)).Select(x => Path.Combine(directorypath, x.FileIdentifier)).ToList();
		}

		public FileReader OpenFile(String filepath, Encoding encoding)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(encoding, nameof(encoding));

			var totalfilepath = Path.Combine(RootPath, filepath);

			var file = Image.GetFile(totalfilepath);
			if (file == null) return null;

			return new FileReader(file.GetData(), encoding);
		}

		public void SaveFile(String filepath, Byte[] buffer)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(buffer, nameof(buffer));

			var totalfilepath = Path.Combine(RootPath, filepath);

			var file = Image.GetFile(totalfilepath);
			if (file == null) return;

			file.Record.DataLength = (UInt32)buffer.Length;
			file.SetData(buffer);

			var sectorsused = MathUtil.RoundUp(buffer.Length, IsoImage.DefaultSectorSize) / IsoImage.DefaultSectorSize;
			if (Image.CheckForRoom(file.Record, sectorsused) == false) Image.ChangeFileSector(file, Image.GetHighestSectorUsed() + 1);
		}

		String WildcardToRegex(String pattern)
		{
			Assert.IsNotNull(pattern, nameof(pattern));

			return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
		}

		IsoImage Image { get; }

		String RootPath { get; }
	}
}