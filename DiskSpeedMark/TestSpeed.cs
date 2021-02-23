using DiskSpeedMark.Database;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace DiskSpeedMark
{
    internal class TestSpeed
    {
        private readonly long fileSize;
        private readonly int numberOfFiles;
        private readonly string driveLetter;
        public TestResult testResult;
        private BackgroundWorker worker;
        ResultsDbContext context;
        Result result;

        public TestSpeed(string driveLetter, long fileSizeInBytes, int numberOfFiles)
        {
            this.driveLetter = driveLetter;
            this.fileSize = fileSizeInBytes;
            this.numberOfFiles = numberOfFiles;
            context = new ResultsDbContext();
            result = new Result(fileSizeInBytes, numberOfFiles);
        }

        public void RunTest(object worker)
        {
            this.worker = (BackgroundWorker)worker;
            this.worker.ReportProgress(0, null);
            WriteTest();
            ReadTest();
            testResult = new TestResult
            {
                AvgWriteSpeed = result.GetAvgWriteSpeed(),
                AvgReadSpeed = result.GetAvgReadSpeed(),
                FinishedAt = DateTime.Now.ToString(),
                DriveName = driveLetter
            };
            DeleteTestFiles();
            SaveResultInDatabase();

            this.worker.ReportProgress(100, result);

        }

        private void WriteTest()
        {
            byte[] testData = GenerateRandomArray(fileSize);
            TimeSpan timeSum = new TimeSpan(0);

            for (int i = 1; i <= numberOfFiles; i++)
            {
                DateTime start = DateTime.Now;

                using (FileStream fileStream = new FileStream($"{ driveLetter }\\TestFile{ i }.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)fileSize, FileOptions.SequentialScan))
                {
                    fileStream.Write(testData, 0, testData.Length);

                    var end = DateTime.Now;
                    timeSum += end - start;
                    result.AddWriteTime(end - start);

                    double progress = (i / (numberOfFiles * 2.0)) * 100.0;
                    worker.ReportProgress((int)progress, result);

                    Thread.Sleep(100);
                }
            }
        }

        public void ReadTest()
        {
            TimeSpan timeSum = new TimeSpan(0);

            for (int i = 1; i <= numberOfFiles; i++)
            {
                byte[] testData = new byte[fileSize];

                var start = DateTime.Now;

                using (FileStream fileStream = new FileStream($"{ driveLetter }\\TestFile{ i }.txt", FileMode.Open, FileAccess.Read, FileShare.Read, (int)fileSize, FileOptions.SequentialScan))
                {
                    fileStream.Read(testData);

                    var end = DateTime.Now;
                    timeSum += end - start;
                    result.AddReadTime(end - start);

                    double progress = (0.5 + (i / (numberOfFiles * 2.0))) * 100.0;
                    worker.ReportProgress((int)progress, result);

                    Thread.Sleep(100);
                }
            }
        }

        private void DeleteTestFiles()
        {
            for (int i = 1; i <= numberOfFiles; i++)
            {
                FileInfo file = new FileInfo($"{ driveLetter }\\TestFile{ i }.txt");
                file.Delete();
            }
        }

        private void SaveResultInDatabase()
        {
            context.TestsResults.Add(testResult);
            context.SaveChanges();
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
