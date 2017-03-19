using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfCopy
{
    /// <summary>
    /// Class for describing the directories states 
    /// </summary>
    public class DirectoryTree
    {
        private static GridView GridView { get; set; }

        /// <summary>
        /// Set view for directory in Grid 
        /// </summary>
        private static void SetDirectoryGridView()
        {
            try
            {
                GridView = new GridView();

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Title",
                    DisplayMemberBinding = new Binding("Name"),
                    Width = 200
                });

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Path",
                    DisplayMemberBinding = new Binding("FullPath"),
                    Width = 200
                });

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Created",
                    DisplayMemberBinding = new Binding("DateOfCreation"),
                    Width = 100
                });

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Size",
                    DisplayMemberBinding = new Binding("Size"),
                    Width = 100
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method returns path to parent directory of selected directory 
        /// </summary>
        /// <param name="path">path to selected directory</param>
        /// <returns></returns>
        public static string GetParentDirectory(string path)
        {
            string parentDirectoryPath = string.Empty;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Parent!=null)
                {
                    parentDirectoryPath = directoryInfo.Parent.FullName;
                }
                else if(new DriveInfo(directoryInfo.FullName).IsReady)
                {
                    parentDirectoryPath = new DriveInfo(directoryInfo.FullName).RootDirectory.FullName;
                }
                else
                {
                    parentDirectoryPath = path;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return parentDirectoryPath;
        }

        /// <summary>
        /// Method describes information about selected directory and 
        /// Opens all directories / files inside 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeCollection"></param>
        public static void GetIntoDirectory(string path, ListView nodeCollection)
        {
            try
            {
                SetDirectoryGridView();

                nodeCollection.Items.Clear();
                nodeCollection.View = GridView;

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    try
                    {
                        if (directory.Attributes != FileAttributes.System)
                        {
                            nodeCollection.Items.Add(new DirectoryInTree
                            {
                                Name = directory.Name,
                                FullPath = directory.FullName,
                                DateOfCreation = directory.CreationTimeUtc.ToShortDateString()
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    try
                    {
                        if (file.Attributes != FileAttributes.System)
                        {
                            nodeCollection.Items.Add(new FileInTree
                            {
                                Name = file.Name, 
                                FullPath = file.FullName, 
                                DateOfCreation = file.CreationTimeUtc.ToShortDateString(), 
                                Size = $"{((double)file.Length / 1024/1024.0f).ToString("0.00")} MB"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }
    }

    /// <summary>
    /// POCO class for files 
    /// </summary>
    public class FileInTree 
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DateOfCreation { get; set; }
        public string Size { get; set; } = string.Empty;
    }

    /// <summary>
    /// POCO class for directories 
    /// </summary>
    public class DirectoryInTree
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DateOfCreation { get; set; }
        public string Size { get; set; } = string.Empty;
    }
}