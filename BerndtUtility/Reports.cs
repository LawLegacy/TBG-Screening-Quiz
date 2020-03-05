using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;

namespace BerndtUtility
{
    public static class Reports
    {
        public static bool HttpGetReport(string sourcePath, string outputFile)
        {
            bool success = false;

            Dictionary<string, int> reportData = null;

            try
            {
                if (File.Exists(sourcePath))
                {
                    // The path is a file
                    reportData = ProcessHttpGetReportFile(sourcePath);
                }
                else if (Directory.Exists(sourcePath))
                {
                    // The path is a directory
                    reportData = ProcessHttpGetReportDirectory(sourcePath);
                }

                if (reportData != null)
                {
                    WriteToCSV(reportData, outputFile);
                }

                success = true;
            }
            catch { }

            return success;
        }

        private static Dictionary<string, int> ProcessHttpGetReportFile(string sourcePath, Dictionary<string, int> requestCount = null)
        {
            if (requestCount == null)
                requestCount = new Dictionary<string, int>();

            foreach (string line in File.ReadLines(sourcePath, Encoding.UTF8))
            {
                // Process each line
                string[] lineData = line.Split(' ');

                // Skip comments
                if (lineData[0].Trim().StartsWith("#"))
                    continue;

                // Check for GET request and from standard HTTP port 80
                if (lineData[7] == "80" && lineData[8].ToUpper() == "GET")
                {
                    // Exclude requests from IP's beginning with '207.114'
                    if (!lineData[2].StartsWith("207.114"))
                    {
                        // Check if request count contains the IP
                        if (!requestCount.ContainsKey(lineData[2]))
                        {
                            requestCount.Add(lineData[2], 0);
                        }

                        // Increment request count for ip
                        requestCount[lineData[2]]++;
                    }
                }
            }

            return requestCount;
        }

        private static Dictionary<string, int> ProcessHttpGetReportDirectory(string sourcePath)
        {
            Dictionary<string, int> requestCount =  new Dictionary<string, int>();

            string[] files = Directory.GetFiles(sourcePath);
            foreach (string fileName in files)
                requestCount =  ProcessHttpGetReportFile(fileName, requestCount);

            return requestCount;
        }

        private static void WriteToCSV(Dictionary<string, int> reportData, string outputFile)
        {
            // Sort values descending
            var sortedDict = reportData.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key, new IPComparer());

            String csv = String.Join(
                Environment.NewLine,
                sortedDict.Select(d => $"{d.Value},{d.Key}")
            );
            
            File.WriteAllText(outputFile, csv);
        }
    }

    public class IPComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return Enumerable.Zip(a.ToString().Split('.'), b.ToString().Split('.'), (x, y) => int.Parse(x).CompareTo(int.Parse(y))).FirstOrDefault(i => i != 0);
        }
    }
}
