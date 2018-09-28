using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GC.WebClock.Utilities
{
    public class CustomLogging
    {
        public static void ErrorLog(Exception exception)
        {
            try
            {
                var assemblyUrl = System.Reflection.Assembly.GetEntryAssembly().Location;
                string logFilePath = System.IO.Path.GetDirectoryName(assemblyUrl) + @"\LogFiles\";
                if (!System.IO.Directory.Exists(logFilePath))
                    System.IO.Directory.CreateDirectory(logFilePath);

                logFilePath = logFilePath + @"Error\";
                if (!System.IO.Directory.Exists(logFilePath))
                    System.IO.Directory.CreateDirectory(logFilePath);

                var path = logFilePath + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";

                using (var file = File.AppendText(path))
                {
                    file.WriteLine("Date & Time: " + DateTime.Now.ToString("F"));
                    file.WriteLine("---------------------------------------------");
                    file.WriteLine("..:: EXCEPTION ::..");
                    file.WriteLine("---------------------------------------------");
                    file.WriteLine();
                    file.WriteLine("Message: " + (string.IsNullOrEmpty(exception.Message) ? "NONE" : exception.Message));
                    file.WriteLine("Source: " + (string.IsNullOrEmpty(exception.Source) ? "NONE" : exception.Source));
                    file.WriteLine("Stack Trace: " + (string.IsNullOrEmpty(exception.StackTrace) ? "NONE" : exception.StackTrace));
                    file.WriteLine();
                    file.WriteLine("---------------------------------------------");
                    file.WriteLine("..:: INNER EXCEPTION ::..");
                    file.WriteLine("---------------------------------------------");
                    file.WriteLine();
                    if (exception.InnerException != null)
                    {
                        file.WriteLine("Message: " + (string.IsNullOrEmpty(exception.InnerException.Message) ? "NONE" : exception.InnerException.Message));
                        file.WriteLine("Source: " + (string.IsNullOrEmpty(exception.InnerException.Source) ? "NONE" : exception.InnerException.Source));
                        file.WriteLine("Stack Trace: " + (string.IsNullOrEmpty(exception.InnerException.StackTrace) ? "NONE" : exception.InnerException.StackTrace));
                        file.WriteLine();
                        file.WriteLine("---------------------------------------------");
                        file.WriteLine("..:: FURTHER INNER EXCEPTION ::..");
                        file.WriteLine("---------------------------------------------");
                        file.WriteLine();
                        if (exception.InnerException.InnerException != null)
                        {
                            file.WriteLine("Message: " + (string.IsNullOrEmpty(exception.InnerException.InnerException.Message) ? "NONE" : exception.InnerException.InnerException.Message));
                            file.WriteLine("Source: " + (string.IsNullOrEmpty(exception.InnerException.InnerException.Source) ? "NONE" : exception.InnerException.InnerException.Source));
                            file.WriteLine("Stack Trace: " + (string.IsNullOrEmpty(exception.InnerException.InnerException.StackTrace) ? "NONE" : exception.InnerException.InnerException.StackTrace));
                        }
                        else
                        {
                            file.WriteLine("NONE");
                        }
                    }
                    else
                    {
                        file.WriteLine("NONE");
                    }
                    file.WriteLine();
                    file.WriteLine("=============================================");
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void InfoLog(string info)
        {
            try
            {
                var assemblyUrl = System.Reflection.Assembly.GetEntryAssembly().Location;
                string logFilePath = System.IO.Path.GetDirectoryName(assemblyUrl) + @"\LogFiles\";
                if (!System.IO.Directory.Exists(logFilePath))
                    System.IO.Directory.CreateDirectory(logFilePath);

                logFilePath = logFilePath + @"Info\";
                if (!System.IO.Directory.Exists(logFilePath))
                    System.IO.Directory.CreateDirectory(logFilePath);

                var path = logFilePath + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";

                using (var file = File.AppendText(path))
                {
                    file.WriteLine("Date & Time: " + DateTime.Now.ToString("F"));
                    file.WriteLine("--------------------------------------------------");
                    file.WriteLine("..::INFO::..");
                    file.WriteLine("--------------------------------------------------");
                    file.WriteLine();
                    file.WriteLine("Detail: " + info);
                    file.WriteLine();
                    file.WriteLine("==================================================");
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
