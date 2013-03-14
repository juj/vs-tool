using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace pythonemcc
{
    class Program
    {
        /// <summary>
        /// This program is a small tool to workaround issues that Visual Studio/MSBuild
        /// has when it tries to invoke 'python emcc' directly. In particular, VS uses
        /// this concept of 'response files', which is a file that stores the command line
        /// input parameters for a process. This application is called by vs-tool to
        /// route commands from VS to python emcc.
        /// 
        /// Ideally, this tool should never exist. If someone can find an alternative way,
        /// I'll be happy to hear it.
        /// </summary>
        /// <remarks>Patched by Fabian Korak</remarks>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            List<string> emccargs = new List<string>();
            string firstparam = args.Length > 0 ? args[0] : "";
            if (firstparam.StartsWith("@")) // Is this a reference to a .rsp file?
            {
                firstparam = firstparam.Substring(1);
                string s = "";
                using (StreamReader rdr = File.OpenText(firstparam))
                    s = rdr.ReadToEnd();
                emccargs.Add(s);
            }
            else // No .rsp file, pass the arguments directly
                emccargs.AddRange(args);
//            Console.WriteLine("Invoking emcc with cmdline: " + emccargs);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            //Get the windows PATH: http://stackoverflow.com/questions/5578385/assembly-searchpath-via-path-environment
            string sysPATH = System.Environment.GetEnvironmentVariable("PATH");
            var parsedPATH = sysPATH.Split(';');

            foreach (var filePATH in parsedPATH)
            {
                //We need to search for something containing python2, so it doesn't work on computers with python3 in the path
                if (Regex.IsMatch(filePATH, @"python2+",RegexOptions.IgnoreCase))
                {
                    psi.FileName = filePATH;
                    //The file needs to point towards python.exe, since we use that directly
                    if (!psi.FileName.EndsWith(".exe"))
                    {
                        psi.FileName += "\\python.exe";
                    }
                    break;
                }

            }

            //If there is nothing with Python in the path just go for a default path
            if (psi.FileName == "")
            {
                if (!File.Exists("c:\\python27\\python.exe"))
                    Console.WriteLine("Error: c:\\python27\\python.exe does not exist! Recompile vs-tool with a proper path to python!");
                psi.FileName = "c:\\python27\\python.exe"; 
            } // On Win7, it seems just having here "python" works, but not on Vista.
            // We assume that the 'emcc' python executable and this application reside in the
            // same directory.
            // http://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-net-in-a-console-app
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);

            var toolname = System.IO.Path.GetFileNameWithoutExtension(path);
            string a = directory + "\\" + toolname;
            foreach(string s in emccargs)
                a += " " + s;

            psi.Arguments = a;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            
            // We don't do anything with the StandardInput redirect, but it exists to workaround Python bugs. See:
            // http://bugs.python.org/issue3905
            // and https://github.com/kripken/emscripten/issues/718
            psi.RedirectStandardInput = true;

            int processReturnCode = 1;
            try
            {
                Process p = Process.Start(psi);
                p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                p.ErrorDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                while (!p.HasExited)
                {
                    p.WaitForExit(10000);
                    if (!p.HasExited)
                        Console.WriteLine(toolname + " running.. please wait.");
                }
                processReturnCode = p.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception received when starting tool '" + psi.FileName + "' with command line '" + psi.Arguments + "'!\n" + e.ToString());
                return 1;
            }

            return processReturnCode;
        }

        static void p_OutputDataReceived(object sender, DataReceivedEventArgs line)
        {
            if (line.Data != null)
            {
                if (line.Data.EndsWith("\n"))
                    Console.Write(line.Data);
                else
                    Console.WriteLine(line.Data);
            }
        }
    }
}
