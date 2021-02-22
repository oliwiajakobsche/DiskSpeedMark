using DiskSpeedMark.Database;
using System;
using System.ComponentModel;
using System.IO;

namespace DiskSpeedMark
{
    internal class TestSpeed
    {
        private readonly long fileSize;
        private readonly int numberOfFiles;
        private readonly string driveLetter;
        public TestResult testResult;
        private BackgroundWorker worker;
        ResultsDbContext context = new ResultsDbContext();

        public TestSpeed(string driveLetter, long fileSizeInBytes, int numberOfFiles)
        {
            this.driveLetter = driveLetter;
            this.fileSize = fileSizeInBytes;
            this.numberOfFiles = numberOfFiles;
        }

        public void RunTest(object worker)
        {
            this.worker = (BackgroundWorker)worker;
            this.worker.ReportProgress(0);
            Result writeSpeedResult = WriteTest();
            Result readSpeedResult = ReadTest();
            testResult = new TestResult
            {
                AvgWriteSpeed = writeSpeedResult.GetAvgSpeed(),
                AvgReadSpeed = readSpeedResult.GetAvgSpeed(),
                FinishedAt = DateTime.Now.ToString(),
                DriveName = driveLetter
            };
            this.worker.ReportProgress(100, testResult);
            DeleteTestFiles();
            SaveResultInDatabase();
        }

        private Result WriteTest()
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

                    double progress = (i / (numberOfFiles * 2.0)) * 100.0;
                    worker.ReportProgress((int)progress);
                }
            }

            return new Result(timeSum, fileSize * numberOfFiles);
        }

        public Result ReadTest()
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

                    double progress = (0.5 + (i / (numberOfFiles * 2.0))) * 100.0;
                    worker.ReportProgress((int)progress);
                }
            }

            return new Result(timeSum, fileSize * numberOfFiles);
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
