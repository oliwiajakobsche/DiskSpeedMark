using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            List<DriveInfo> drives = DriveInfo.GetDrives().ToList();

            _drivesListCb.Items.Clear();

            foreach(var drive in drives)
            {
                _drivesListCb.Items.Add($"{ drive.Name } { drive.VolumeLabel }");
            }

        }


        private void _drivesListCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_drivesListCb.SelectedItem != null)
                {
                    DriveInfo selectedDrive = DriveInfo.GetDrives()
                                              .Where(x => new String($"{ x.Name } { x.VolumeLabel }") == _drivesListCb.SelectedItem.ToString())
                                              .FirstOrDefault();

                    _volumeNameTb.Text = selectedDrive.VolumeLabel;
                    _formatTb.Text = selectedDrive.DriveFormat;

                    double BytesInGigabyte = 8.0 * 1024.0 * 1024.0 * 1024.0;
                    _totalSpaceTb.Text = ((double)selectedDrive.TotalSize / BytesInGigabyte).ToString("##0.00") + " GB";
                    _availableSpaceTb.Text = ((double)selectedDrive.AvailableFreeSpace / BytesInGigabyte).ToString("##0.00") + " GB";
                }
            }
            catch
            {
                MessageBox.Show("Sorry, but it seems you disconnected this disk or it is unreachable.");
            }
            
        }
    }
}
