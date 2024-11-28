using FileSystemVisitor1;
using System;
using System.Collections.Generic;
using System.IO;

public class FileSystemVisitor
{
    private readonly string _rootFolder;
    private readonly Func<string, bool> _filter;
    private bool _abortRequested = false;

    public event EventHandler Start;
    public event EventHandler Finish;
    public event EventHandler<FileSystemVisitorEventArgs> FileFound;
    public event EventHandler<FileSystemVisitorEventArgs> DirectoryFound;
    public event EventHandler<FileSystemVisitorEventArgs> FilteredFileFound;
    public event EventHandler<FileSystemVisitorEventArgs> FilteredDirectoryFound;
    public FileSystemVisitor(string rootFolder)
    {
        _rootFolder = rootFolder ?? throw new ArgumentNullException(nameof(rootFolder));
        _filter = null;
    }

    public FileSystemVisitor(string rootFolder, Func<string, bool> filter) : this(rootFolder)
    {
        _filter = filter ?? throw new ArgumentNullException(nameof(filter));
    }

    public IEnumerable<string> Traverse()
    {
        Start?.Invoke(this, EventArgs.Empty);

        foreach (var item in TraverseDirectory(_rootFolder))
        {
            if (_abortRequested)
                break;

            yield return item;
        }

        Finish?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerable<string> TraverseDirectory(string directoryPath)
    {
        // invoke all directories
        bool excludeDirectory = false;
        var dirArgs = new FileSystemVisitorEventArgs { Path = directoryPath };
        DirectoryFound?.Invoke(this, dirArgs);
        if (dirArgs.Exclude) excludeDirectory = true;
        if (dirArgs.Abort) _abortRequested = true;

        if (!excludeDirectory && (_filter == null || _filter(directoryPath)) && Directory.Exists(directoryPath))
        {
            // invoke only filtered directories
            var filteredDirArgs = new FileSystemVisitorEventArgs { Path = directoryPath };
            FilteredDirectoryFound?.Invoke(this, filteredDirArgs);

            if (!filteredDirArgs.Exclude)
                yield return directoryPath;

            if (filteredDirArgs.Abort)
                _abortRequested = true;
        }

        if (!_abortRequested && Directory.Exists(directoryPath))
        {
            IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(directoryPath);
            foreach (string entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    foreach (var subItem in TraverseDirectory(entry))
                        yield return subItem;
                }
                else
                {
                    // invoke all files
                    bool excludeFile = false;
                    var fileArgs = new FileSystemVisitorEventArgs { Path = entry };
                    FileFound?.Invoke(this, fileArgs);
                    if (fileArgs.Exclude) excludeFile = true;
                    if (fileArgs.Abort) _abortRequested = true;

                    // invoke only filtered files
                    if (!excludeFile && (_filter == null || _filter(entry)))
                    {
                        var filteredFileArgs = new FileSystemVisitorEventArgs { Path = entry };
                        FilteredFileFound?.Invoke(this, filteredFileArgs);

                        if (!filteredFileArgs.Exclude)
                            yield return entry;

                        if (filteredFileArgs.Abort)
                        {
                            _abortRequested = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}