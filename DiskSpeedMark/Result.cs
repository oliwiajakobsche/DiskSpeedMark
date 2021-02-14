namespace DiskSpeedMark
{
    internal class Result
    {
        public decimal TimeInSeconds { get; set; }
        public decimal Megabytes { get; set; }

        public Result(long time, double bytes)
        {
            this.TimeInSeconds = (decimal)time/1000;
            this.Megabytes = (decimal)(bytes/1024/1024);
        }

        public decimal GetAvgSpeed()
        {
            decimal avgSpeed = Megabytes / TimeInSeconds;

            return avgSpeed;
        }
    }
}
