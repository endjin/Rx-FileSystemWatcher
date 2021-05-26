namespace RxFileSystemWatcher
{
	using System.IO;

	public class FileDropped
	{
		public FileDropped(FileSystemEventArgs fileEvent)
		{
			this.Name = fileEvent.Name;
			this.FullPath = fileEvent.FullPath;
		}

		public FileDropped(string filePath)
		{
			this.Name = Path.GetFileName(filePath);
			this.FullPath = filePath;
		}

		public string Name { get; private set; }

		public string FullPath { get; private set; }
	}
}