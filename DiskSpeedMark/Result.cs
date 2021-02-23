using System;
using System.Collections.Generic;
using System.Text;

namespace DiskSpeedMark
{
    class Result
    {
        private List<decimal> readTimeMsSamples { get; set; }
        private List<decimal> writeTimeMsSamples { get; set; }
        public decimal totalSizeInMegabytes { get; set; }
        public List<decimal> readSpeedSamplesMBps { get; }

        public List<decimal> writeSpeedSamplesMBps { get; }
        public decimal fileSizeInMegabytes { get; set; }

        public Result(double bytes, int numberOfFiles)
        {
            fileSizeInMegabytes = (decimal)(bytes / 1024 / 1024);
            totalSizeInMegabytes = ((decimal)(bytes * numberOfFiles / 1024 / 1024));
            readTimeMsSamples = new List<decimal>();
            writeTimeMsSamples = new List<decimal>();
            writeSpeedSamplesMBps = new List<decimal>();
            readSpeedSamplesMBps = new List<decimal>();
        }

        public void AddWriteTime(TimeSpan writeTime)
        {
            writeTimeMsSamples.Add((decimal)writeTime.TotalMilliseconds);
            decimal avgSampleSpeed = Math.Round((fileSizeInMegabytes / ((decimal)writeTime.TotalMilliseconds / 1000)), 2);
            writeSpeedSamplesMBps.Add(avgSampleSpeed);
        }
        public void AddReadTime(TimeSpan readTime)
        {
            readTimeMsSamples.Add((decimal)readTime.TotalMilliseconds);
            decimal avgSampleSpeed = Math.Round((fileSizeInMegabytes / ((decimal)readTime.TotalMilliseconds / 1000)), 2);
            readSpeedSamplesMBps.Add(avgSampleSpeed);
        }

        public decimal GetAvgReadSpeed()
        {
            decimal sumaricTimeInMs = 0;

            foreach (decimal sample in readTimeMsSamples) //more accurate than summing averages from readSpeedSamplesMBps
            {
                sumaricTimeInMs += sample;
            }

            decimal avgSpeed = Math.Round((totalSizeInMegabytes / (sumaricTimeInMs/1000)), 2);

            return avgSpeed;
        }
        public decimal GetAvgWriteSpeed()
        {
            decimal sumaricTimeInMs = 0;

            foreach (decimal sample in writeTimeMsSamples)
            {
                sumaricTimeInMs += sample;
            }

            decimal avgSpeed = Math.Round((totalSizeInMegabytes / (sumaricTimeInMs/1000)), 2);

            return avgSpeed;
        }

    }
}
