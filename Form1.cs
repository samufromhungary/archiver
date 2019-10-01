using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanityArchiver
{
    public partial class filewalker : Form
    {
        public FileInfo SelectedFile { get; set; }
        public string currentPath { get; set; }
        public DirectoryInfo SelectedDirectory { get; set; }

        private ListViewItem SelectedItem
        {
            get
            {
                if (listViewFiles.SelectedItems.Count > 0)
                {
                    return listViewFiles.SelectedItems[0];
                }
                else
                    return null;
            }
        }

        public filewalker()
        {
            InitializeComponent();
            currentPath = @"C:\";
        }

        public void ShowFiles(string currentPath)
        {
            listViewFiles.Items.Clear();
            FileHandler fileHandler = new FileHandler(currentPath);

            try
            {
                foreach (var item in fileHandler.Directories)
                {
                    string[] details = { item.Name, "DIR", item.LastAccessTime.ToShortDateString(), item.Attributes.ToString() };
                    ListViewItem listViewItem = new ListViewItem(details);
                    listViewFiles.Items.Add(listViewItem);
                }

                foreach (var item in fileHandler.Files)
                {
                    string[] details = { item.Name, item.Extension, item.LastAccessTime.ToShortDateString(), item.Attributes.ToString(), (item.Length / 1024).ToString() };
                    ListViewItem listViewItem = new ListViewItem(details);
                    listViewFiles.Items.Add(listViewItem);
                }

                foreach (var item in fileHandler.Files)
                {
                    string[] details = { item.Name, item.Extension, item.LastAccessTime.ToShortDateString(), item.Attributes.ToString(), (item.Length / 1024).ToString() };
                    ListViewItem listViewItem = new ListViewItem(details);

                }

            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Cannot access:\nError: {ex.Message}\n\n" +
                        $"Info:\n{ex.StackTrace}");
            }

        }
        public void Archive(FileInfo fileToArchive, string currentPath)
        {
            using (FileStream input = fileToArchive.OpenRead())
            {
                FileStream output = File.Create(currentPath + @"\" + fileToArchive.Name + ".gz");
                GZipStream compressor = new GZipStream(output, CompressionMode.Compress);
                int b = input.ReadByte();

                while (b != -1)
                {
                    compressor.WriteByte((byte)b);
                    b = input.ReadByte();
                }
            }
        }

        private void ZipFileButton_Click(object sender, EventArgs e)
        {
            Archive(SelectedFile, currentPath);
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                SelectedFile = new FileInfo(SelectedItem.Text);
            }catch(NullReferenceException ex)
            {
                MessageBox.Show($"Cannot access:\nError: {ex.Message}\n\n" +
        $"Info:\n{ex.StackTrace}");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ColumnHeader col1 = new ColumnHeader();
            col1.Text = "File Name";
            col1.Name = "name";
            listViewFiles.Columns.Add(col1);

            ColumnHeader col2 = new ColumnHeader();
            col2.Text = "File Extension";
            col2.Name = "extension";
            listViewFiles.Columns.Add(col2);

            ShowFiles(currentPath);
        }

        private void listViewFiles_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem selectedItem = listViewFiles.SelectedItems[0];
            if (selectedItem.SubItems[1].Text == "DIR")
            {
                SelectedDirectory = new DirectoryInfo(currentPath + @"\" + selectedItem.Text);
                currentPath = currentPath + @"\" + selectedItem.Text;
                labelPath.Text = currentPath.ToString();
                ShowFiles(currentPath);
            }
            else
            {
                SelectedFile = new FileInfo(currentPath + @"\" + selectedItem.Text);
                System.Diagnostics.Process.Start(SelectedFile.FullName);
            }
        }

        private void backButton_Click_1(object sender, EventArgs e)
        {
            if (currentPath != Path.GetPathRoot(Environment.SystemDirectory))
            {
                if (SelectedDirectory != null && new DirectoryInfo(SelectedDirectory.Parent.FullName) != null)
                {
                    SelectedDirectory = new DirectoryInfo(SelectedDirectory.Parent.FullName);
                    currentPath = SelectedDirectory.FullName;
                    labelPath.Text = currentPath.ToString();
                    ShowFiles(currentPath);
                }
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select path" };
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                currentPath = fbd.SelectedPath.ToString();
                labelPath.Text = currentPath.ToString();
                InitializeComponent();
            }
        }

        private void BtnRoot_Click(object sender, EventArgs e)
        {
            if(currentPath == @"C:\")
            {
                currentPath = @"D:\";
            }
            else
            {
                currentPath = @"C:\";
            }
            labelPath.Text = currentPath.ToString();
            ShowFiles(currentPath);
        }
    }
}
