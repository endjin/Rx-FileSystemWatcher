namespace RxFileSystemWatcher
{
	using System;
	using System.IO;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;

	/// <summary>
	/// An observable abstraction to monitor for files dropped into a directory
	/// </summary>
	public class FileDropWatcher : IDisposable
	{
		private readonly string path;
		private readonly string filter;
		private readonly ObservableFileSystemWatcher watcher;
		private readonly Subject<FileDropped> pollResults = new Subject<FileDropped>();

		public IObservable<FileDropped> Dropped { get; private set; }

		public FileDropWatcher(string path, string filter)
		{
			this.path = path;
			this.filter = filter;
			this.watcher = new ObservableFileSystemWatcher(w =>
			{
				w.Path = path;
				w.Filter = filter;
				// note: filtering on changes can help reduce excessive notifications, make sure to verify any changes with integration tests
				w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
			});

			var renames = this.watcher.Renamed.Select(r => new FileDropped(r));
			var creates = this.watcher.Created.Select(c => new FileDropped(c));
			var changed = this.watcher.Changed.Select(c => new FileDropped(c));

			this.Dropped = creates
				.Merge(renames)
				.Merge(changed)
				.Merge(this.pollResults);
		}

		public void Start()
		{
			this.watcher.Start();
		}

		public void Stop()
		{
			this.watcher.Stop();
		}

		public void Dispose()
		{
			this.watcher.Dispose();
		}

		/// <summary>
		/// Use this to scan for files and raise dropped events for any results.
		/// This is great to use right after starting the watcher to find existing files.
		/// Existing files will trigger dropped events through the Dropped stream.
		/// </summary>
		public void PollExisting()
		{
            foreach (var existingFile in Directory.GetFiles(this.path, this.filter))
            {
                this.pollResults.OnNext(new FileDropped(existingFile));
            }
        }
	}
}