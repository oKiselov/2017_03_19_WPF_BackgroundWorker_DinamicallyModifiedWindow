using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfCopy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Child window for describing the process of file's copying 
        /// </summary>
        private WindowCopy _copyWindow = null;

        /// <summary>
        /// Default constructor 
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                DriveTree.SetDrives(ListViewDestinationFrom);

                DriveTree.SetDrives(ListViewDestinationTo);

                ButtonBackUpFrom.Content = string.Empty;

                ButtonBackUpTo.Content = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// EventHandler runs when all processes are finished 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CopyWindowOnFinished(object sender, EventArgs args)
        {
            try
            {
                _copyWindow = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// EventHandler for COPY button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartCopy_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathToDirectory = ((DirectoryInTree)ListViewDestinationTo.Items[0]).FullPath;

                if (ListViewDestinationFrom.SelectedItems.Count == 0)
                {
                    throw new Exception("Please, select files to copy");
                }

                if (!new DirectoryInfo(pathToDirectory).Parent.Exists &&
                    (new DirectoryInfo(pathToDirectory).Parent.Attributes & FileAttributes.System) != 0)
                {
                    throw new Exception("Please, select correct directory to copy in");
                }

                List<string> listPathesOfFiles = new List<string>();

                for (int i = 0; i < ListViewDestinationFrom.SelectedItems.Count; i++)
                {
                    if (!new FileInfo(((FileInTree)ListViewDestinationFrom.SelectedItems[i]).FullPath).Exists)
                    {
                        throw new Exception("Please, select correct file to copy from directory");
                    }

                    if (
                        new FileInfo(
                            $"{new DirectoryInfo(pathToDirectory).Parent.FullName}\\{new FileInfo(((FileInTree)ListViewDestinationFrom.SelectedItems[i]).FullPath).Name}")
                            .Exists)
                    {
                        throw new Exception($"File: {new DirectoryInfo(pathToDirectory).Parent.FullName}\\{new FileInfo(((FileInTree)ListViewDestinationFrom.SelectedItems[i]).FullPath).Name} has already existed");

                    }

                    listPathesOfFiles.Add(((FileInTree)ListViewDestinationFrom.SelectedItems[i]).FullPath);
                }

                // Initialization of WindowCopy or addition to it new UIElements 
                if (_copyWindow == null)
                {
                    _copyWindow = new WindowCopy();
                    _copyWindow.Finished += CopyWindowOnFinished;
                    _copyWindow.Show();
                }
              
                // Run the copy processes for each file 
                foreach (var file in listPathesOfFiles)
                {
                    _copyWindow.AddProgressWindow(file, new DirectoryInfo(pathToDirectory).Parent.FullName);
                    _copyWindow.StartCopy(file, new DirectoryInfo(pathToDirectory).Parent.FullName);
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("You have selected the folder, but should select files");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles behavior of applecation if button BACK is pushed in ListViewDestinationFrom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBackUpFrom_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ButtonBackUpFrom.Content.ToString()))
                {
                    DirectoryTree.GetIntoDirectory(ButtonBackUpFrom.Content.ToString(), ListViewDestinationFrom);

                    ButtonBackUpFrom.Content =
                        string.Format(DirectoryTree.GetParentDirectory(ButtonBackUpFrom.Content.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles behavior of applecation if button BACK is pushed in ListViewDestinationTo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBackUpTo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ButtonBackUpTo.Content.ToString()))
                {
                    DirectoryTree.GetIntoDirectory(ButtonBackUpTo.Content.ToString(), ListViewDestinationTo);

                    ButtonBackUpTo.Content =
                        string.Format(DirectoryTree.GetParentDirectory(ButtonBackUpTo.Content.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles MOUSE DOUBLE CLICK for ListViewDestinationFrom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDestinationFrom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PushSelected(ListViewDestinationFrom, ButtonBackUpFrom);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles MOUSE DOUBLE CLICK for ListViewDestinationTo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDestinationTo_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PushSelected(ListViewDestinationTo, ButtonBackUpTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method show new state of ListView after item's selection 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="button"></param>
        private void PushSelected(ListView listView, Button button)
        {
            try
            {
                if (listView.SelectedItem is DriveTree)
                {
                    button.Content =
                        DirectoryTree.GetParentDirectory(((DriveTree)listView.SelectedItem).Name);

                    DirectoryTree.GetIntoDirectory(((DriveTree)listView.SelectedItem).Name,
                        listView);
                }
                else if (listView.SelectedItem is DirectoryInTree)
                {
                    button.Content =
                        DirectoryTree.GetParentDirectory(
                            ((DirectoryInTree)listView.SelectedItem).FullPath);

                    DirectoryTree.GetIntoDirectory(((DirectoryInTree)listView.SelectedItem).FullPath,
                        listView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method backs ListView to drives 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonToDrivesFrom_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DriveTree.SetDrives(ListViewDestinationFrom);
                ButtonBackUpFrom.Content = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method backs ListView to drives 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonToDrivesTo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DriveTree.SetDrives(ListViewDestinationTo);
                ButtonBackUpTo.Content = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles the Enter Key Pushing on the items of ListViewDestinationFrom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDestinationFrom_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (sender is ListView && e.Key == Key.Enter)
                {
                    PushSelected(ListViewDestinationFrom, ButtonBackUpFrom);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method handles the Enter Key Pushing on the items of ListViewDestinationTo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDestinationTo_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (sender is ListView && e.Key == Key.Enter)
                {
                    PushSelected(ListViewDestinationTo, ButtonBackUpTo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}