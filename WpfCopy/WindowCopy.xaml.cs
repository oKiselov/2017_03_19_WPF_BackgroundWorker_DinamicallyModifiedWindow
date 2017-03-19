using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfCopy
{
    /// <summary>
    /// Interaction logic for WindowCopy.xaml
    /// </summary>
    public partial class WindowCopy : Window
    {
        /// <summary>
        /// Delegate and event, which send to main window information about abscence of executing copy processes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void InitFinished(object sender, EventArgs args);

        public event InitFinished Finished;

        /// <summary>
        /// List of UIElements for each copy process 
        /// </summary>
        public List<ProgressBarWindowSettings> ListSettings = new List<ProgressBarWindowSettings>();

        /// <summary>
        /// List of separators 
        /// </summary>
        public List<Separator> ListSeparators = new List<Separator>(); 

        public WindowCopy()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method runs when COPY-window sends to main window information about abscence of executing copy processes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnFinishedProcess(object sender, EventArgs args)
        {
            try
            {
                ProgressBarWindowSettings progressBarWindow = (ProgressBarWindowSettings) sender;
                
                // find sender as UIElement of main StackPanel 
                int index = ListSettings.IndexOf(progressBarWindow);

                StackPanelMain.Children.Remove(ListSettings[index].GridMain);
                StackPanelMain.Children.Remove(ListSeparators[index]);

                // remove information and modify StackPanel 
                ListSettings[index] = null;
                ListSettings.RemoveAt(index);

                ListSeparators[index] = null; 
                ListSeparators.RemoveAt(index);
                
                if (Height - 155 >= 155)
                {
                    MaxHeight -= 155;
                    Height -= 155;
                }

                // if all processes are finished - close window 
                if (ListSettings.Count == 0)
                {
                    Close();
                    Finished(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method runs when USER pushes button COPY 
        /// Method modifies StackPanel, adds to it new UIElement - copying content 
        /// And sign for UIElement new EventHAndler - OnFinishedProcess 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="pathToDirectory"></param>
        public void AddProgressWindow(string pathToFile, string pathToDirectory)
        {
            try
            {
                ListSettings.Add(new ProgressBarWindowSettings());
                ListSeparators.Add(new Separator());
                MaxHeight += 155;
                Height += 155;

                ListSettings[ListSettings.Count - 1].TextBlockFrom.Text = string.Format($"FROM:\t{pathToFile}\r\nTO:\t{pathToDirectory}");
                ListSettings[ListSettings.Count - 1].TextBlockFrom.Margin = new Thickness(5,5,5,5);

                StackPanelMain.Children.Add(ListSeparators[ListSeparators.Count-1]);
                StackPanelMain.Children.Add(ListSettings[ListSettings.Count - 1].GridMain);
                ListSettings[ListSettings.Count - 1].FinishedProcess += OnFinishedProcess; 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Method runs current copy process using BackGroundWorker async method 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="pathDirection"></param>
        public void StartCopy(string pathToFile, string pathDirection)
        {
            try
            {
                PathesToCopy pathes = new PathesToCopy { File = pathToFile, Directory = pathDirection};
                ListSettings[ListSettings.Count-1].BackgroundWorker.RunWorkerAsync(pathes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Close all copying processes before window bacomes closed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowCopy_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                foreach (ProgressBarWindowSettings setting in ListSettings)
                {
                    setting.BackgroundWorker.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
