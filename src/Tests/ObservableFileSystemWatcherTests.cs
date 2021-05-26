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
	public class ObservableFileSystemWatcherTests : FileIntegrationTestsBase
	{
		[Test]
		[Timeout(2000)]
		public async Task WriteToFile_StreamsChanged()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstChanged = watcher.Changed.FirstAsync().ToTask();
				watcher.Start();

				File.WriteAllText(Path.Combine(TempPath, "Changed.Txt"), "foo");

				var changed = await firstChanged;
				Expect(changed.ChangeType).To.Equal(WatcherChangeTypes.Changed);
				Expect(changed.Name).To.Equal("Changed.Txt");
			}
		}

		[Test]
		[Timeout(2000)]
		public async Task CreateFile_StreamsCreated()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstCreated = watcher.Created.FirstAsync().ToTask();
				var filePath = Path.Combine(TempPath, "Created.Txt");
				watcher.Start();

				File.WriteAllText(filePath, "foo");

				var created = await firstCreated;
				Expect(created.ChangeType).To.Equal(WatcherChangeTypes.Created);
				Expect(created.Name).To.Equal("Created.Txt");
			}
		}

		[Test]
		[Timeout(2000)]
		public async Task DeleteFile_StreamsDeleted()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstDeleted = watcher.Deleted.FirstAsync().ToTask();
				var filePath = Path.Combine(TempPath, "ToDelete.Txt");
				File.WriteAllText(filePath, "foo");
				watcher.Start();

				File.Delete(filePath);

				var deleted = await firstDeleted;
				Expect(deleted.ChangeType).To.Equal(WatcherChangeTypes.Deleted);
				Expect(deleted.Name).To.Equal("ToDelete.Txt");
			}
		}

		[Test]
		[Timeout(2000)]
		public async Task DeleteMonitoredDirectory_StreamsError()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstError = watcher.Errors.FirstAsync().ToTask();
				watcher.Start();

				Directory.Delete(TempPath);

				var error = await firstError;
				Expect(error.GetException().Message).To.Equal("Access is denied.");
			}
		}

		[Test]
		[Timeout(2000)]
		public async Task RenameFile_StreamsRenamed()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstRenamed = watcher.Renamed.FirstAsync().ToTask();
				var originalPath = Path.Combine(TempPath, "Changed.Txt");
				File.WriteAllText(originalPath, "foo");
				watcher.Start();

				var renamedPath = Path.Combine(TempPath, "Renamed.Txt");
				File.Move(originalPath, renamedPath);

				var renamed = await firstRenamed;
				Expect(renamed.OldFullPath).To.Equal(originalPath);
				Expect(renamed.FullPath).To.Equal(renamedPath);
			}
		}
	}
}