using System;
using System.Collections.Generic;
using System.Text;

namespace DiskSpeedMark
{
    internal class Result
    {
        public long timeInSeconds { get; set; }
        public double megabytes { get; set; }

        public Result(long time, double bytes)
        {
            this.timeInSeconds = time/1000;
            this.megabytes = bytes/1024/1024;
        }

        public double GetAvgSpeed()
        {
            double avgSpeed = megabytes / timeInSeconds;

            return avgSpeed;
        }
    }
}
