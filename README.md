# RxFileSystemWatcher

A reactive wrapper around FileSystemWatcher.

## Contents

- `ObservableFileSystemWatcher` - an observable wrapper around the FileSystemWatcher type
- `FileDropWatcher` - an observable stream of events to detect when a file has been dropped in a directory

## Building

```
dotnet build -c release ./src/RxFileSystemWatcher.sln
```

## Testing

This project has a suite of integration tests to verify the behaviour of monitoring the file system. The tests are a great way to understand the behaviours.

```
dotnet test -c release ./src/RxFileSystemWatcher.sln
```

## Packaging

```
dotnet pack -c release ./src/RxFileSystemWatcher.sln
```

## History

- Jan 07, 2014 - Initial Release from [Wes Higbee](https://github.com/g0t4)
- May 26, 2021 - [endjin](https://endjin.com) updates project to .NET Standard 2.0.

## Licenses

[![GitHub license](https://img.shields.io/badge/MIT-blue.svg)](./LICENSE)

RxFileSystemWatcher is available under the [MIT open source license](./LICENSE).