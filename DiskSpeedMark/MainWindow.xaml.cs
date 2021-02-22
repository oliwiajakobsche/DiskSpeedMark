using DiskSpeedMark.Database;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DiskSpeedMark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     
        public MainWindow()
        {
            InitializeComponent();
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

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _testProgressBar.Value = e.ProgressPercentage;

            TestResult result = (TestResult)e.UserState;

            if (e.UserState != null)
            {
                _avgWriteSpeedTb.Content = result.AvgWriteSpeed.ToString("#0.00") + " MB/s";
                _avgReadSpeedTb.Content = result.AvgReadSpeed.ToString("#0.00") + " MB/s";
                _avgWriteSpeedPb.Value = (double)result.AvgWriteSpeed / _avgWriteSpeedPb.Maximum;
                _avgReadSpeedPb.Value = (double)result.AvgReadSpeed / _avgReadSpeedPb.Maximum;
                _startTestBt.IsEnabled = true;
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
