using DiskSpeedMark.Database;
using System.Linq;
using System.Windows;

namespace DiskSpeedMark
{
    /// <summary>
    /// Interaction logic for TestResultsHistoryWindow.xaml
    /// </summary>
    public partial class TestResultsHistoryWindow : Window
    {
        public TestResultsHistoryWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            ResultsDbContext dbcontext = new ResultsDbContext();
            historyData.ItemsSource = dbcontext.TestsResults.ToList();
            dbcontext.Dispose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
