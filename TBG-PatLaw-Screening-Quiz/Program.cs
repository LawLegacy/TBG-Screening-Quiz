using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BerndtUtility;

namespace TBG_PatLaw_Screening_Quiz
{
    class Program
    {
        static void Main(string[] args)
        {
            //Reports.HttpGetReport(ConfigurationManager.AppSettings["LogFile"], ConfigurationManager.AppSettings["OutputFile"]);
            Reports.HttpGetReport(ConfigurationManager.AppSettings["LogDirectory"], ConfigurationManager.AppSettings["OutputFile"]);
        }
    }
}
