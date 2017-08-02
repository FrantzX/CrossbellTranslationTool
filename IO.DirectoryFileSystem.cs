using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace CrossbellTranslationTool.IO
{
	class DirectoryFileSystem : IFileSystem
	{
		public DirectoryFileSystem(String rootpath)
		{
			Assert.IsValidString(rootpath, nameof(rootpath));

			RootPath = rootpath;
		}

		public Boolean DoesFileExist(String filepath)
		{
			Assert.IsValidString(filepath, nameof(filepath));

			var totalfilepath = Path.Combine(RootPath, filepath);

			return File.Exists(totalfilepath);
		}

		public List<String> GetChildren(String directorypath, String searchpattern)
		{
			Assert.IsValidString(directorypath, nameof(directorypath));
			Assert.IsValidString(searchpattern, nameof(searchpattern));

			var totaldirectorypath = Path.Combine(RootPath, directorypath);

			var children = Directory.GetFiles(totaldirectorypath, searchpattern);

			return children.Select(x => GetRelativePath(x)).ToList();
		}

		public FileReader OpenFile(String filepath, Encoding encoding)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(encoding, nameof(encoding));

			var totalfilepath = Path.Combine(RootPath, filepath);
			var buffer = File.ReadAllBytes(totalfilepath);

			return new FileReader(buffer, encoding);
		}

		public void SaveFile(String filepath, Byte[] buffer)
		{
			Assert.IsValidString(filepath, nameof(filepath));
			Assert.IsNotNull(buffer, nameof(buffer));

			var totalfilepath = Path.Combine(RootPath, filepath);

			File.Delete(totalfilepath);
			File.WriteAllBytes(totalfilepath, buffer);
		}

		String GetRelativePath(String fullpath)
		{
			Assert.IsValidString(fullpath, nameof(fullpath));

			var root = RootPath;
			if (root.Length > 0 && root[root.Length - 1] != '\\') root = root + @"\";

			if (fullpath.StartsWith(root, StringComparison.OrdinalIgnoreCase) == true)
			{
				return fullpath.Substring(root.Length);
			}
			else
			{
				return fullpath;
			}
		}

		String RootPath { get; }
	}
}