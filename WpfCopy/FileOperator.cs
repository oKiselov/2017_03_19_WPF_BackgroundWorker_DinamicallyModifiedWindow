using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;

namespace WpfCopy
{
    /// <summary>
    /// POCO class for BackGroundWorker's Run method 
    /// </summary>
    public class PathesToCopy
    {
        public string File { get; set; }

        public string Directory { get; set; }
    }

    /// <summary>
    /// Class for operations with classes 
    /// </summary>
    public class FileOperator
    {
        /// <summary>
        /// Static class for file's copying 
        /// </summary>
        /// <param name="pathToFile">path to file</param>
        /// <param name="pathDirection">path to directory</param>
        /// <param name="worker">ref to BackGroundWorker</param>
        /// <param name="manualEvent">ref to ManualResetEvent </param>
        public static void CopyFile(string pathToFile, string pathDirection, BackgroundWorker worker, ManualResetEvent manualEvent)
        {
            try
            {
                using (FileStream streamRead = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
                {
                    using (
                        FileStream streamWrite = new FileStream($"{pathDirection}\\{new FileInfo(pathToFile).Name}",
                            FileMode.Create, FileAccess.Write))
                    {

                        long lProgressFroWorker = 0;

                        long lPosInReadFile = 0;
                        long lPosInWriteFile = 0;
                        byte[] arrBytes = new byte[(int) ClusterSize.Small];
                        BinaryReader reader = new BinaryReader(streamRead);

                        reader.BaseStream.Seek(lPosInReadFile, SeekOrigin.Begin);

                        manualEvent.Set();

                        while (lPosInReadFile < streamRead.Length)
                        {
                            // wait if button Pause is pushed 
                            manualEvent.WaitOne();
                            
                            arrBytes = reader.ReadBytes(arrBytes.Length);
                            lPosInReadFile += arrBytes.Length;

                            if (lPosInWriteFile < streamRead.Length)
                            {
                                BinaryWriter writer = new BinaryWriter(streamWrite);
                                writer.Write(arrBytes);
                                lPosInWriteFile += arrBytes.Length;
                            }

                            // information for BackGroundWorker and ProgressBar
                            if (lPosInReadFile*100/streamRead.Length > lProgressFroWorker && worker != null)
                            {
                                lProgressFroWorker = lPosInReadFile*100/streamRead.Length;

                                if (worker.CancellationPending)
                                {
                                    return;
                                }

                                if (worker.WorkerReportsProgress)
                                {
                                    worker.ReportProgress((int) lProgressFroWorker);
                                }
                            }
                        }
                        if (worker.WorkerReportsProgress)
                        {
                            worker.ReportProgress(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}