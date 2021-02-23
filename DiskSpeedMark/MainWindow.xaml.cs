using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace DiskSpeedMark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<KeyValuePair<int, decimal>> ChartSeriesWrite;
        public ObservableCollection<KeyValuePair<int, decimal>> ChartSeriesRead;
        int temp = 0;

        public MainWindow()
        {
            InitializeComponent();

            ChartSeriesWrite = new ObservableCollection<KeyValuePair<int, decimal>>();
            ChartSeriesRead = new ObservableCollection<KeyValuePair<int, decimal>>();
            (chartSeriesWrite as LineSeries).ItemsSource = ChartSeriesWrite;
            (chartSeriesRead as LineSeries).ItemsSource = ChartSeriesRead;
        }

        private void _drivesListCb_DropDownOpened(object sender, EventArgs e)
        {
            _drivesListCb.ItemsSource = Drive.RefreshedList();
        }

        private void _drivesListCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_drivesListCb.SelectedItem != null)
                {
                    Drive.ChooseDrive(_drivesListCb.SelectedItem.ToString());
                    _volumeNameTb.Text = Drive.Name;
                    _formatTb.Text = Drive.Format;
                    _totalSpaceTb.Text = Drive.TotalSpaceInGb.ToString("##0.00") + " GB";
                    _availableSpaceTb.Text = Drive.FreeSpaceInGb.ToString("##0.00") + " GB";
                }
                else
                {
                    _volumeNameTb.Text = "---";
                    _formatTb.Text = "---";
                    _totalSpaceTb.Text = "---";
                    _availableSpaceTb.Text = "---";
                }
                RefreshGuiValues();
            }
            catch
            {
                MessageBox.Show("Sorry, but it seems you disconnected this disk or it is unreachable.");
            }
        }

        private void RefreshGuiValues()
        {
            _avgWriteSpeedTb.Content = "-/- MB/s";
            _avgReadSpeedTb.Content = "-/- MB/s";
            _avgWriteSpeedPb.Value = 0;
            _avgReadSpeedPb.Value = 0;
            _testProgressBar.Value = 0;
        }



        private void _startTestBt_Click(object sender, RoutedEventArgs e)
        {
            _startTestBt.IsEnabled = false;
            RefreshGuiValues();
            DrawAChart(null);

            if (_drivesListCb.SelectedItem != null && !string.IsNullOrWhiteSpace(_sizeOfFileTb.Text) && _sizeUnitCb.SelectedItem != null && !string.IsNullOrWhiteSpace(_numberOfTestsTb.Text))
            {
                long sizeOfFile = long.Parse(_sizeOfFileTb.Text);
                long sizeInBytes = 1;

                try
                {
                    switch (_sizeUnitCb.SelectedValue.ToString())
                    {
                        case "B":
                            sizeInBytes = 1 * sizeOfFile;
                            break;
                        case "KB":
                            sizeInBytes = 1024 * sizeOfFile;
                            break;
                        case "MB":
                            sizeInBytes = 1024 * 1024 * sizeOfFile;
                            break;
                        case "GB":
                            sizeInBytes = 1024 * 1024 * 1024 * sizeOfFile;
                            break;
                    }

                    TestSpeed test = new TestSpeed(Drive.Letter, sizeInBytes, int.Parse(_numberOfTestsTb.Text));

                    #region Background worker
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += Worker_DoWork;
                    worker.ProgressChanged += Worker_ProgressChanged;
                    worker.WorkerReportsProgress = true;
                    worker.RunWorkerCompleted += Worker_Completed;
                    worker.RunWorkerAsync(test);
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error happened while testing in main UI thread\n { ex.Message} ");
                }
            }
            else
            {
                MessageBox.Show("Choose parameters.");
            }
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            temp = 0;
        }

        private void DrawAChart(Result result)
        {
            if (temp == 0)
            {
                ChartSeriesRead.Clear();
                ChartSeriesWrite.Clear();
                temp++;
            }
            if (result != null)
            {
                int writeSamples = result.writeSpeedSamplesMBps.Count;
                if (writeSamples > 0 && writeSamples > ChartSeriesWrite.Count)
                {
                    int testFileNumber = writeSamples - 1;
                    decimal avgSpeed = result.writeSpeedSamplesMBps[testFileNumber];
                    ChartSeriesWrite.Add(new KeyValuePair<int, decimal>(testFileNumber+1, avgSpeed));
                }

                int readSamples = result.readSpeedSamplesMBps.Count;
                if (readSamples > 0 && readSamples > ChartSeriesRead.Count)
                {
                    int testFileNumber = readSamples - 1;
                    decimal avgSpeed = result.readSpeedSamplesMBps[testFileNumber];
                    ChartSeriesRead.Add(new KeyValuePair<int, decimal>(testFileNumber+1, avgSpeed));
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Result result = (Result)e.UserState;
            _testProgressBar.Value = e.ProgressPercentage;

            if (e.UserState != null)
            {
                DrawAChart(result);

                if (result.writeSpeedSamplesMBps.Count > 0)
                {
                    _avgWriteSpeedTb.Content = result.GetAvgWriteSpeed().ToString("#0.00") + " MB/s";
                    _avgWriteSpeedPb.Value = (double)result.GetAvgWriteSpeed() / _avgWriteSpeedPb.Maximum;
                }
                if (result.readSpeedSamplesMBps.Count > 0)
                {
                    _avgReadSpeedTb.Content = result.GetAvgReadSpeed().ToString("#0.00") + " MB/s";
                    _avgReadSpeedPb.Value = (double)result.GetAvgReadSpeed() / _avgReadSpeedPb.Maximum;
                }
                if (result.readSpeedSamplesMBps.Count == int.Parse(_numberOfTestsTb.Text))
                {
                    _startTestBt.IsEnabled = true;
                }
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TestSpeed test = (TestSpeed)e.Argument;
                test.RunTest(sender);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while performing tests in background worker thread.\n\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void _sizeUnitCb_Initialized(object sender, EventArgs e)
        {
            _sizeUnitCb.Items.Add("B");
            _sizeUnitCb.Items.Add("KB");
            _sizeUnitCb.Items.Add("MB");
            _sizeUnitCb.Items.Add("GB");
            _sizeUnitCb.SelectedIndex = 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestResultsHistoryWindow testResults = new TestResultsHistoryWindow();
            this.Visibility = Visibility.Hidden;
            testResults.Show();
        }
    }
}
