using System;

namespace DiskSpeedMark
{
    internal class Result
    {
        public decimal TimeInSeconds { get; set; }
        public decimal Megabytes { get; set; }

        public Result(TimeSpan time, double bytes)
        {
            this.TimeInSeconds = (decimal)time.TotalSeconds;
            this.Megabytes = (decimal)(bytes / 1024 / 1024);
        }

        public decimal GetAvgSpeed()
        {
            decimal avgSpeed = Math.Round((Megabytes / TimeInSeconds), 2);

            return avgSpeed;
        }
    }
}
