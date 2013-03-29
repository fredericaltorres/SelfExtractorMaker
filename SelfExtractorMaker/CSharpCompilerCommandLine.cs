using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSugar;

namespace SelfExtractorMaker
{
    public enum TargetType {
        exe,
        winexe,
        library 
    }
    /// <summary>
    /// csc.exe /debug /define:DEBUG /target:exe /out:VrmAgentInstaller.exe /resource:VrmAgentInstaller.msi SelfExtractor.cs
    /// csc.exe /target:exe /out:VrmAgentInstaller.exe /resource:VrmAgentInstaller.msi SelfExtractor.cs
    /// </summary>
    class CSharpCompilerCommandLine {

        public TargetType Target;
        public bool Debug;
        public string OutputFile;
        public string CSharpSourceWildCard;
        public List<string> ResourceFiles = new List<string>();

        public CSharpCompilerCommandLine() {

        }

        private string GetCommandLine() {

            var b = new StringBuilder(1000);

            if(this.Debug)
                b.Append("/debug /define:DEBUG ");

            b.AppendFormat(@"/target:{0} ", this.Target);
            b.AppendFormat(@"""/out:{0}"" ", this.OutputFile);

            foreach(var r in this.ResourceFiles)
                b.AppendFormat(@"""/resource:{0}"" ", r);

            b.AppendFormat(@"""{0}""", this.CSharpSourceWildCard);

            return b.ToString();
        }

        public ExecutionInfo Compile() {

            return ExecuteProgramWithCapture("csc.exe", this.GetCommandLine());
        }

        public class ExecutionInfo
        {
            public string   Output;
            public string   ErrorOutput;
            public int      Time;
            public string   CommandLine;
            public int      ErrorLevel;

            public bool Succeeded {
                get {
                    return this.ErrorLevel == 0;
                }
            }

            public ExecutionInfo() {

              this.Output      = "";
              this.ErrorOutput = "";
              this.Time        = -1;
              this.CommandLine = "";
              this.ErrorLevel  = -1;
            }
        }
        
        private static ExecutionInfo ExecuteProgramWithCapture(string program, string commandLine)
        {
            var e                     = new ExecutionInfo();
            e.Time                    = Environment.TickCount;
            e.ErrorLevel              = -1;
            StreamReader outputReader = null;
            StreamReader errorReader  = null;
            try
            {
                ProcessStartInfo processStartInfo       = new ProcessStartInfo(program, commandLine);
                processStartInfo.ErrorDialog            = false;
                processStartInfo.UseShellExecute        = false;
                processStartInfo.RedirectStandardError  = true;
                processStartInfo.RedirectStandardInput  = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow         = false;
                Process process                         = new Process();
                process.StartInfo                       = processStartInfo;
                bool processStarted                     = process.Start();

                if (processStarted)
                {
                    outputReader  = process.StandardOutput;
                    errorReader   = process.StandardError;
                    process.WaitForExit();
                    e.Output      = outputReader.ReadToEnd();
                    e.ErrorOutput = errorReader.ReadToEnd();
                    e.ErrorLevel  = process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                e.ErrorOutput += "Error lanching the nodejs.exe = {0}".format(ex.ToString());
            }
            finally
            {
                if (outputReader != null)
                    outputReader.Close();
                if (errorReader != null)
                    errorReader.Close();
            }
            e.Time = Environment.TickCount - e.Time;
            return e;
        }
    }
}
