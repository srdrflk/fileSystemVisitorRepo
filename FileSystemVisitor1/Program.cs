

FileSystemVisitor visitor = new FileSystemVisitor(@"C:\Users\Serdar_Filik\Desktop\EPAM__\KATYA", file => file.EndsWith(".pdf"));

visitor.Start += (sender, args) => Console.WriteLine("Search started.");
visitor.Finish += (sender, args) => Console.WriteLine("Search finished.");

visitor.FileFound += (sender, args) => Console.WriteLine("File found: " + args.Path);
visitor.FilteredFileFound += (sender, args) => Console.Write("Filtered file found: ");

visitor.DirectoryFound += (sender, args) => Console.WriteLine("Directory found: " + args.Path);
visitor.FilteredDirectoryFound += (sender, args) => Console.WriteLine("Filtered directory found: " + args.Path);


foreach (var file in visitor.Traverse())
{
    Console.WriteLine(file);
}