namespace RxFileSystemWatcher
{
    using System;
    using System.IO;
    using System.Reactive.Linq;

    /// <summary>
    /// This is a wrapper around a file system watcher to use the Rx framework instead of event handlers to handle
    /// notifications of file system changes.
    /// </summary>
    public class ObservableFileSystemWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;

        public IObservable<FileSystemEventArgs> Created { get; private set; }

        public IObservable<FileSystemEventArgs> Changed { get; private set; }

        public IObservable<FileSystemEventArgs> Deleted { get; private set; }

        public IObservable<ErrorEventArgs> Errors { get; private set; }

        public IObservable<RenamedEventArgs> Renamed { get; private set; }

        /// <summary>
        /// Pass an existing FileSystemWatcher instance, this is just for the case where it's not possible to only pass the
        /// configuration, be aware that disposing this wrapper will dispose the FileSystemWatcher instance too.
        /// </summary>
        /// <param name="watcher"></param>
        public ObservableFileSystemWatcher(FileSystemWatcher watcher)
        {
            this.watcher = watcher;

            this.Changed = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => this.watcher.Changed += h, h => this.watcher.Changed -= h)
                .Select(x => x.EventArgs);

            this.Renamed = Observable
                .FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => this.watcher.Renamed += h, h => this.watcher.Renamed -= h)
                .Select(x => x.EventArgs);

            this.Deleted = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => this.watcher.Deleted += h, h => this.watcher.Deleted -= h)
                .Select(x => x.EventArgs);

            this.Errors = Observable
                .FromEventPattern<ErrorEventHandler, ErrorEventArgs>(h => this.watcher.Error += h, h => this.watcher.Error -= h)
                .Select(x => x.EventArgs);

            this.Created = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => this.watcher.Created += h, h => this.watcher.Created -= h)
                .Select(x => x.EventArgs);
        }

        /// <summary>
        /// Pass a function to configure the FileSystemWatcher as desired, this constructor will manage creating and applying
        /// the configuration.
        /// </summary>
        public ObservableFileSystemWatcher(Action<FileSystemWatcher> configure) : this(new FileSystemWatcher())
        {
            configure(this.watcher);
        }

        public void Start()
        {
            this.watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            this.watcher.EnableRaisingEvents = false;
        }

        public void Dispose()
        {
            this.watcher.Dispose();
        }
    }
}