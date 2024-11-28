using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemVisitor1
{
    public class FileSystemVisitorEventArgs : EventArgs
    {
        public string Path { get; set; }
        public bool Exclude { get; set; } = false;
        public bool Abort { get; set; } = false;
    }
}
