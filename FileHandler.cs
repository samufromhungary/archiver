using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanityArchiver
{
    public class FileHandler
    {
        public string currentPath { get; set; }
        public DirectoryInfo[] Directories { get; set; }
        public FileInfo[] Files { get; set; }

        public FileHandler (string currentPath)
        {
            currentPath = currentPath;
            try
            {
                Directories = new DirectoryInfo(currentPath).GetDirectories();
                Files = new DirectoryInfo(currentPath).GetFiles();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Cannot access:\nError: {ex.Message}\n\n" +
                        $"Info:\n{ex.StackTrace}");
            }
        }
    }
}
