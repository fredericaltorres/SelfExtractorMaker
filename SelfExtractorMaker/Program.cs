using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DynamicSugar;

namespace SelfExtractorMaker {

    class Program {

        const string CommandLineHelp = @"
Parameters:
    -exe OutputExeName 
    -program Executable to run after extraction [optional]
    -arguments Command line parameter for the executable [optional]
    [-keepsource] if passed the C# file generated is not deleted    
    -files List of file to embed [must be the last parameter defined in the command line]

Samples:

SelfExtractorMaker.exe -exe MyInstaller.exe -program msiexec.exe -arguments ""/i MyInstaller.msi"" -files MyInstaller.msi
";
        /// <summary>
        /// SelfExtractorMaker.exe -exe VrmAgentInstaller.exe -program msiexec.exe -arguments "/i {DQ}{Path}\VrmAgentInstaller.msi{DQ}" -files VrmAgentInstaller.msi DemInstaller.msi
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            var commandLine = new Arguments(args);

            Console.WriteLine("{0} - v {1}".format(Application.ProductName, Application.ProductVersion));

            var v = commandLine.ValidateRequiredParameter("-exe", "-files");
            if(!v.Succeeded) {
                Console.WriteLine(v.Message);
                Console.WriteLine(CommandLineHelp);
                System.Environment.Exit(1);
            }
            
            var keepSourceTmpCSharpFile = commandLine.Exists("-keepsource");
            var cSharpTmpFileName       = "cs.cs";            
            var exeOutputFileName       = commandLine.GetArgument("-exe");
            var program                 = commandLine.GetArgument("-program");
            var arguments               = commandLine.GetArgument("-arguments");
            var files                   = commandLine.GetAll("-files");
            var cSharpTemplate          = DynamicSugar.DS.Resources.GetTextResource("ExtractorConsoleTemplate.cs", Assembly.GetExecutingAssembly());
            cSharpTemplate              = cSharpTemplate.Replace("[EXE]", program).
                                                         Replace("[ARGUMENTS]", arguments).
                                                         Replace("/*[FILES]*/", files.Format());
                        
            Console.WriteLine("Creating self-extracting {0}".format(exeOutputFileName));
            Console.WriteLine("Embeding {0}".format(files.Format()));
            Console.WriteLine("Action {0} {1}".format(program, arguments));

            File.WriteAllText(cSharpTmpFileName, cSharpTemplate);

            try {
                var c                  = new CSharpCompilerCommandLine();
                c.Target               = TargetType.exe;
                c.OutputFile           = exeOutputFileName;
                c.CSharpSourceWildCard = "cs.cs";
                c.ResourceFiles        = files;
                var e                  = c.Compile();
                Console.WriteLine(e.Succeeded ? "{0} Created".format(exeOutputFileName)
                                              : "Error compiling {0}\n{1}\n{2}".format(e.ErrorLevel, e.Output, e.ErrorOutput));
            }
            finally {
                if(!keepSourceTmpCSharpFile)
                    System.IO.File.Delete(cSharpTmpFileName);
            }
        }
    }
}
