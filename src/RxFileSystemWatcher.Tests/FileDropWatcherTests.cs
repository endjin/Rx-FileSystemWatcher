namespace Tests
{
	using System.IO;
	using System.Reactive.Linq;
	using System.Reactive.Threading.Tasks;
	using System.Threading.Tasks;
	using NUnit.Framework;
	using RxFileSystemWatcher;
	using NExpect;

	using static NExpect.Expectations;

	[TestFixture]
	public class FileDropWatcherTests : FileIntegrationTestsBase
	{
		[Test]
		[Timeout(2000)]
		public async Task FileDropped_NoExistingFile_StreamsDropped()
		{
			using var watcher = new FileDropWatcher(this.TempPath, "Monitored.Txt");
			var firstDropped = watcher.Dropped.FirstAsync().ToTask();
			watcher.Start();

			var monitoredFile = Path.Combine(this.TempPath, "Monitored.Txt");
			await File.WriteAllTextAsync(monitoredFile, "foo");

			var dropped = await firstDropped;
			Expect(dropped.Name).To.Equal("Monitored.Txt");
			Expect(dropped.FullPath).To.Equal(monitoredFile);
		}

		[Test]
		[Timeout(2000)]
		public async Task FileRenamed_NoExistingFile_StreamsDropped()
		{
			using var watcher = new FileDropWatcher(this.TempPath, "Monitored.Txt");
			var firstDropped = watcher.Dropped.FirstAsync().ToTask();
			var otherFile = Path.Combine(this.TempPath, "Other.Txt");
			await File.WriteAllTextAsync(otherFile, "foo");
			watcher.Start();

			var monitoredFile = Path.Combine(this.TempPath, "Monitored.Txt");
			File.Move(otherFile, monitoredFile);

			var dropped = await firstDropped;
			Expect(dropped.Name).To.Equal("Monitored.Txt");
			Expect(dropped.FullPath).To.Equal(monitoredFile);
		}

		[Test]
		[Timeout(2000)]
		public async Task Overwrite_ExistingFile_StreamsDropped()
		{
			using var watcher = new FileDropWatcher(this.TempPath, "Monitored.Txt");
			var firstDropped = watcher.Dropped.FirstAsync().ToTask();
			var monitoredFile = Path.Combine(this.TempPath, "Monitored.Txt");
			await File.WriteAllTextAsync(monitoredFile, "foo");
			watcher.Start();

			await File.WriteAllTextAsync(monitoredFile, "bar");

			var dropped = await firstDropped;
			Expect(dropped.Name).To.Equal("Monitored.Txt");
			Expect(dropped.FullPath).To.Equal(monitoredFile);
		}

		[Test]
		[Timeout(2000)]
		public async Task PollExisting_FileBeforeStart_StreamsDropped()
		{
			using var watcher = new FileDropWatcher(this.TempPath, "Monitored.Txt");
			var firstDropped = watcher.Dropped.FirstAsync().ToTask();
			var monitoredFile = Path.Combine(this.TempPath, "Monitored.Txt");
			await File.WriteAllTextAsync(monitoredFile, "foo");

			watcher.PollExisting();

			var dropped = await firstDropped;
			Expect(dropped.Name).To.Equal("Monitored.Txt");
			Expect(dropped.FullPath).To.Equal(monitoredFile);
		}

		[Test]
		[Timeout(2000)]
		public async Task PollExisting_SecondTime_StreamsSecondTime()
		{
			using var watcher = new FileDropWatcher(this.TempPath, "Monitored.Txt");
			var secondDropped = watcher.Dropped.Skip(1).FirstAsync().ToTask();
			var monitoredFile = Path.Combine(this.TempPath, "Monitored.Txt");
			await File.WriteAllTextAsync(monitoredFile, "foo");

			watcher.PollExisting();
			watcher.PollExisting();

			var dropped = await secondDropped;
			Expect(dropped.Name).To.Equal("Monitored.Txt");
			Expect(dropped.FullPath).To.Equal(monitoredFile);
		}
	}
}