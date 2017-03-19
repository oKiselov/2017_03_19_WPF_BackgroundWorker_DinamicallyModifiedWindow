using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfCopy
{
    /// <summary>
    /// Class for describing the drive's states 
    /// </summary>
    public class DriveTree
    {
        public string Name { get; set; }
        public string TotalSize { get; set; }
        public string FreeSize { get; set; }
        private static GridView GridView { get; set; }


        /// <summary>
        /// Method sets view of columns in GridView for drives 
        /// </summary>
        private static void SetDrivesGridView()
        {
            try
            {
                GridView = new GridView();

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Title",
                    DisplayMemberBinding = new Binding("Name"),
                    Width = 100
                });

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Total Size",
                    DisplayMemberBinding = new Binding("TotalSize"),
                    Width = 100
                });

                GridView.Columns.Add(new GridViewColumn
                {
                    Header = "Free Size",
                    DisplayMemberBinding = new Binding("FreeSize"),
                    Width = 100
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method puts information about READY drives into specified ListView 
        /// </summary>
        /// <param name="nodeCollection">Specified ListView </param>
        public static void SetDrives(ListView nodeCollection)
        {
            try
            {
                SetDrivesGridView();

                nodeCollection.Items.Clear();
                nodeCollection.View = GridView;
                
                List<DriveInfo> arrDrives = DriveInfo.GetDrives().ToList();

                foreach (var drive in arrDrives)
                {
                    if (drive.IsReady)
                    {
                        nodeCollection.Items.Add(new DriveTree
                        {
                            Name = drive.Name, 
                            TotalSize = string.Format($"{((double)drive.TotalSize / 1024/ 1024 / 1024.0f).ToString("0.00")} GB"), 
                            FreeSize = string.Format($"{((double)drive.AvailableFreeSpace / 1024/ 1024 / 1024.0f).ToString("0.00")} GB")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}