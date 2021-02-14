using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace DiskSpeedMark
{
    internal class TestSpeed
    {
        private readonly long fileSize;
        private readonly int numberOfFiles;
        private readonly string driveLetter;
        public Result writeSpeedResult;
        public Result readSpeedResult;
        BackgroundWorker worker;

        public TestSpeed(string driveLetter, long fileSizeInBytes, int numberOfFiles, ProgressBar progressBar)
        {
            this.driveLetter = driveLetter;
            this.fileSize = fileSizeInBytes;
            this.numberOfFiles = numberOfFiles;
        }

        public void RunTest(object worker)
        {
            this.worker = (BackgroundWorker)worker;
            this.worker.ReportProgress(0);
            writeSpeedResult = WriteTest();
            readSpeedResult = ReadTest();
            this.worker.ReportProgress(100, new FinalResult { AvgWriteSpeed = WriteSpeedAvgResult(), AvgReadSpeed = ReadSpeedAvgResult()});
        }

        private Result WriteTest()
        {
            byte[] testData = GenerateRandomArray(fileSize);
            Stopwatch sw = new Stopwatch();
            long sumTimes = 0;

            for(int i = 1; i<=numberOfFiles; i++)
            {
                FileStream fileStream = new FileStream($"{ driveLetter }\\TestFile{ i }.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)(fileSize), FileOptions.SequentialScan);
                
                sw.Restart();
                fileStream.Write(testData, 0, testData.Length);
                sw.Stop();

                double progress = (i / (numberOfFiles*2.0)) * 100.0;
                worker.ReportProgress((int)progress);

                sumTimes += sw.ElapsedMilliseconds;

                fileStream.Flush();
                fileStream.Close();
            }

            return new Result(sumTimes, fileSize * numberOfFiles);
        }

        public Result ReadTest()
        {
            Stopwatch sw = new Stopwatch();
            long sumTimes = 0;


            for (int i = 1; i <= numberOfFiles; i++)
            {
                FileStream fileStream = new FileStream($"{ driveLetter }\\TestFile{ i }.txt", FileMode.Open, FileAccess.Read, FileShare.Read, (int)(fileSize), FileOptions.SequentialScan);

                sw.Restart();
                using var sr = new StreamReader(fileStream, Encoding.UTF8);
                string content = sr.ReadToEnd();
                sw.Stop();

                double progress = (0.5 + (i / (numberOfFiles * 2.0))) * 100.0;
                worker.ReportProgress((int)progress);

                sumTimes += sw.ElapsedMilliseconds;

                fileStream.Flush();
                fileStream.Close();
            }

            return new Result(sumTimes, fileSize * numberOfFiles);
        }

        public decimal WriteSpeedAvgResult()
        {
            return writeSpeedResult.GetAvgSpeed();
        }
        public decimal ReadSpeedAvgResult()
        {
            return readSpeedResult.GetAvgSpeed();
        }

        public byte[] GenerateRandomArray(long fileSizeInBytes)
        {
            byte[] randoms = new byte[fileSizeInBytes];

            Random random = new Random();
            for (int i = 0; i < randoms.Length; i++)
            {
                randoms[i] = (byte)random.Next(255);
            }

            return randoms;
        }
    }
}
