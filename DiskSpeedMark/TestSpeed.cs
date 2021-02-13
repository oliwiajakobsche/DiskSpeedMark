using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace DiskSpeedMark
{
    internal class TestSpeed
    {
        private readonly long fileSize;
        private readonly int numberOfFiles;
        private readonly string driveLetter;
        public Result writeSpeedResult;
        ProgressBar progressBar;

        public TestSpeed(string driveLetter, long fileSizeInBytes, int numberOfFiles, ProgressBar progressBar)
        {
            this.driveLetter = driveLetter;
            this.fileSize = fileSizeInBytes;
            this.numberOfFiles = numberOfFiles;
            this.progressBar = progressBar;
        }

        public void RunTest()
        {
            writeSpeedResult = WriteTest();
        }

        public Result WriteTest()
        {
            byte[] testData = GenerateRandomArray(fileSize);
            Stopwatch sw = new Stopwatch();
            long sumTimes = 0;

            progressBar.Value = 0;

            for(int i = 0; i<numberOfFiles; i++)
            {
                FileStream fileStream = new FileStream($"{ driveLetter }\\Temp\\TestFile{ i }.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)(fileSize), FileOptions.SequentialScan);
                
                sw.Restart();
                fileStream.Write(testData, 0, testData.Length);
                sw.Stop();

                progressBar.Value = i + 1 / numberOfFiles * 100;
                sumTimes += sw.ElapsedMilliseconds;

                fileStream.Flush();
                fileStream.Close();
            }

            return new Result(sumTimes, fileSize * numberOfFiles); ;
        }

        public void ReadTest()
        {
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
