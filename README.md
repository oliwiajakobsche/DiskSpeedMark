# DiskSpeedMark

WPF .NET application for testing sequential write and read disk speed.

User need to choose drive to benchmark and test options (size of file and number of files (test tries)).

**Functionalities of the app (what the app really does?)**
* displays list of of mounted drives
* displays basic information regarding to choosen disk - volume label and name, format (NTFS/FaT etc.), total size (in GB), available free space (in GB).
* generates in backend random array with selected size in field "Size of file"
* writes sequentially to disk N times and computes average speed for each trial
* reads sequentially to disk N times and computes average speed for each trial
* removes test data from disk
* plots new data on a chart dynamically after each trial
* saves final result to table in sqlite database
* displays table with results from database


*Screenshot 1 - DiskSpeedMark example test result*

![Alt text](/Screenshots/Screen1.png?raw=true "Optional Title")


*Screenshot 2 - Display of current data during the test and progress of test*

![Alt text](/Screenshots/Screen2.png?raw=true "Display of current data during the test and progress of test")


*Screenshot 3 - Display test results from database*

![Alt text](/Screenshots/Screen3.png?raw=true "Display test results from database")
