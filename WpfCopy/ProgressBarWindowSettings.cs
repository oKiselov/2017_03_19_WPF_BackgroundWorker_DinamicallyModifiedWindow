using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfCopy
{

    /// <summary>
    /// Class for UI Elements for each copy process 
    /// </summary>
    public class ProgressBarWindowSettings
    {
        public BackgroundWorker BackgroundWorker { get; set; }

        public Grid GridMain { get; protected set;}

        public TextBlock TextBlockFrom { get; protected set; }

        public ProgressBar ProgressBarCopy { get; protected set; }

        public Button ButtonPause { get; protected set; }

        public Button ButtonCancel { get; protected set; }

        private ManualResetEvent _eventBusy = new ManualResetEvent(false);

        public delegate void CopyProcessFinished(object sender, EventArgs args);

        public event CopyProcessFinished FinishedProcess; 

        private bool IsPaused { get; set; }

        /// <summary>
        /// Default Constructor 
        /// </summary>
        public ProgressBarWindowSettings()
        {
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.DoWork+= BackgroundWorkerOnDoWork;
            BackgroundWorker.ProgressChanged+= BackgroundWorkerOnProgressChanged;
            BackgroundWorker.RunWorkerCompleted+= BackgroundWorkerOnRunWorkerCompleted;
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true; 

            GridMain = new Grid
            {
                Height = 150,
                RowDefinitions = { new RowDefinition(), new RowDefinition(), new RowDefinition() },
                ColumnDefinitions = { new ColumnDefinition{ Width  = new GridLength(160)}, new ColumnDefinition(), new ColumnDefinition() , new ColumnDefinition {Width = new GridLength(160)} },
            };

            
            TextBlockFrom = new TextBlock();
            Grid.SetColumn(TextBlockFrom, 0);
            Grid.SetRow(TextBlockFrom, 0);
            Grid.SetColumnSpan(TextBlockFrom, 4);
            TextBlockFrom.FontSize = 10;
            TextBlockFrom.FontWeight = FontWeights.Bold;
            TextBlockFrom.TextWrapping = TextWrapping.Wrap;
            GridMain.Children.Add(TextBlockFrom);

            ProgressBarCopy = new ProgressBar();
            Grid.SetColumn(ProgressBarCopy, 0);
            Grid.SetRow(ProgressBarCopy, 1);
            Grid.SetColumnSpan(ProgressBarCopy, 4);
            ProgressBarCopy.Margin = new Thickness(5,5,5,5);
            GridMain.Children.Add(ProgressBarCopy);

            ButtonPause = new Button();
            Grid.SetColumn(ButtonPause, 0);
            Grid.SetRow(ButtonPause, 2);
            ButtonPause.Margin = new Thickness(5, 5, 5, 5);
            ButtonPause.Content = "Pause";
            ButtonPause.Background  = Brushes.BurlyWood;
            ButtonPause.Foreground = Brushes.White;
            ButtonPause.FontWeight = FontWeights.Bold;
            ButtonPause.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonPauseOnClick));
            GridMain.Children.Add(ButtonPause);

            ButtonCancel = new Button();
            Grid.SetColumn(ButtonCancel, 3);
            Grid.SetRow(ButtonCancel, 2);
            ButtonCancel.Margin = new Thickness(5, 5, 5, 5);
            ButtonCancel.Content = "Cancel";
            ButtonCancel.Background = Brushes.BurlyWood;
            ButtonCancel.Foreground = Brushes.White;
            ButtonCancel.FontWeight = FontWeights.Bold;
            ButtonCancel.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonCancelOnClick));
            GridMain.Children.Add(ButtonCancel);
        }

        /// <summary>
        /// Method runs when button "PAUSE" or "RESUME" is pushed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void ButtonPauseOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                bool bPause = IsPaused;

                if (bPause == false)
                {
                    IsPaused = true;
                    ButtonPause.Content = string.Format("Resume");
                    _eventBusy.Reset(); 
                }
                else
                {
                    IsPaused = false;
                    ButtonPause.Content = string.Format("Pause");
                    _eventBusy.Set();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Button runs cancellation mode 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void ButtonCancelOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                BackgroundWorker.CancelAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method runs when COPY process is done 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="runWorkerCompletedEventArgs"></param>
        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            try
            {
                if (runWorkerCompletedEventArgs.Cancelled)
                {
                    ProgressBarCopy.Value = 0;
                    FinishedProcess(this, EventArgs.Empty);
                    return;
                }

                if (runWorkerCompletedEventArgs.Error != null)
                {
                    throw new Exception(runWorkerCompletedEventArgs.Error.Message);
                }
                ProgressBarCopy.Value = 100; 

                FinishedProcess(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method runs when FileOperator.CopyFile sends messages about progress in copy process 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressChangedEventArgs"></param>
        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            try
            {
                ProgressBarCopy.Value = progressChangedEventArgs.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method runs copy process 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="doWorkEventArgs"></param>
        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                PathesToCopy pathes = (PathesToCopy) doWorkEventArgs.Argument; 

                FileOperator.CopyFile(pathes.File, pathes.Directory, BackgroundWorker, _eventBusy);

                if (BackgroundWorker.CancellationPending)
                {
                    doWorkEventArgs.Cancel = true;
                    return;
                }

                doWorkEventArgs.Result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}