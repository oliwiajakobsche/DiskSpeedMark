using System;
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
            }
            catch
            {
                MessageBox.Show("Sorry, but it seems you disconnected this disk or it is unreachable.");
            }            
        }

        private void _startTestBt_Click(object sender, RoutedEventArgs e)
        {
            if (_drivesListCb.SelectedItem != null && !string.IsNullOrWhiteSpace(_sizeOfFileTb.Text) && _sizeUnitCb.SelectedItem != null && !string.IsNullOrWhiteSpace(_numberOfTestsTb.Text))
            {
                long sizeOfFile = long.Parse(_sizeOfFileTb.Text);
                long sizeInBytes = 1;
                string bla = _sizeUnitCb.SelectedItem.ToString();

                try
                {
                    switch(_sizeUnitCb.SelectedValue.ToString())
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

                    var test = new TestSpeed(Drive.Letter, sizeInBytes, int.Parse(_numberOfTestsTb.Text), _testProgressBar);
                    test.RunTest();
                    double AvgSpeed = test.writeSpeedResult.GetAvgSpeed();
                    _resultsTb.Text += "Avg speed (mb/s): " + AvgSpeed.ToString("#0.00");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error happened while testing \n { ex.Message} ");
                }

                _startTestBt.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Choose parameters.");
            }
        }

        private void _sizeUnitCb_Initialized(object sender, EventArgs e)
        {
            _sizeUnitCb.Items.Add("B");
            _sizeUnitCb.Items.Add("KB");
            _sizeUnitCb.Items.Add("MB");
            _sizeUnitCb.Items.Add("GB");
        }
    }
}
