using System.ComponentModel.DataAnnotations.Schema;

namespace DiskSpeedMark.Database
{
    [Table("TestResult")]
    public class TestResult
    {
        public int Id { get; set; }
        public decimal AvgReadSpeed { get; set; }
        public decimal AvgWriteSpeed { get; set; }
        public string FinishedAt { get; set; }
        public string DriveName { get; set; }
    }
}
