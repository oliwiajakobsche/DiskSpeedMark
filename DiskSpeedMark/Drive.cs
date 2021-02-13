using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskSpeedMark
{
    internal static class Drive
    {
        public static string Letter { get; set; }
        public static string Name { get; set; }
        public static string Format { get; set; }
        public static double TotalSpaceInGb { get; set; }
        public static double FreeSpaceInGb { get; set; }

        public static void ChooseDrive(string letterAndVolumeName)
        {
            DriveInfo selectedDrive = DriveInfo.GetDrives()
                                             .Where(x => new String($"{ x.Name } { x.VolumeLabel }") == letterAndVolumeName)
                                             .FirstOrDefault();


            double BytesInGigabyte = 8.0 * 1024.0 * 1024.0 * 1024.0;

            Letter = selectedDrive.Name;
            Name = selectedDrive.VolumeLabel;
            Format = selectedDrive.DriveFormat;
            TotalSpaceInGb = ((double)selectedDrive.TotalSize / BytesInGigabyte);
            FreeSpaceInGb = ((double)selectedDrive.AvailableFreeSpace / BytesInGigabyte);
        }

        public static List<string> RefreshedList()
            {
                List<DriveInfo> drives = DriveInfo.GetDrives().ToList();
                List<string> drivesList = new List<string>();

                foreach (var drive in drives)
                {
                    drivesList.Add($"{ drive.Name } { drive.VolumeLabel }");
                }

                return drivesList;
            }
    }
}
