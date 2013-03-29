using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Drawing;

namespace DynamicSugar {
    /// <summary>
    /// Dynamic Sharp Helper Class, dedicated methods to work with text resource file
    /// </summary>
    public static partial class Embed {

        public static class Resources {
            /// <summary>
            /// Return the fully qualified name of the resource file
            /// </summary>
            /// <param name="resourceFileName">File name of the resource</param>
            /// <returns></returns>
            private static string GetResourceFullName(string resourceFileName, Assembly assembly) {
                
                foreach (var resource in assembly.GetManifestResourceNames())
                    if (resource.EndsWith(resourceFileName))
                        return resource;

                throw new System.ApplicationException(string.Format("Resource '{0}' not find in assembly '{1}'", resourceFileName, Assembly.GetExecutingAssembly().FullName));
            }
            /// <summary>
            /// Return the content of a file embed as a resource.
            /// The function takes care of finding the fully qualify name, in the current
            /// assembly.
            /// </summary>
            /// <param name="resourceFileName"></param>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static byte[] GetBinaryResource(string resourceFileName, Assembly assembly)
            {
                var resourceFullName = GetResourceFullName(resourceFileName, assembly);            
                var stream           = assembly.GetManifestResourceStream(resourceFullName);
                byte[]  data         = new Byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
            /// <summary>
            /// Save a buffer of byte into a file
            /// </summary>
            /// <param name="byteArray"></param>
            /// <param name="fileName"></param>
            /// <returns></returns>
            private static bool SaveByteArrayToFile(byte[] byteArray, string fileName) {
                try {
                    using (Stream fileStream = File.Create(fileName)) {
                        fileStream.Write(byteArray, 0, byteArray.Length);
                        fileStream.Close();
                        return true;
                    }
                }
                catch{
                    return false;
                }
            }
            /// <summary>
            /// Save a resource as a local file
            /// </summary>
            /// <param name="resourceFileName">Resource name and filename</param>
            /// <param name="assembly">Assembly where to get the resource</param>
            /// <param name="path">Local folder</param>
            /// <returns></returns>
            public static string SaveBinaryResourceAsFile(Assembly assembly, string path, string resourceFileName) {

                var outputFileName = System.IO.Path.Combine(path, resourceFileName);
                if (System.IO.File.Exists(outputFileName))
                {
                    System.IO.File.Delete(outputFileName);
                }

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                var buffer = GetBinaryResource(resourceFileName, assembly);

                SaveByteArrayToFile(buffer, outputFileName);
                return outputFileName;
            }
        }
    }
}

namespace SelfExtrator
{
    class SelfExtractor
    {
        static void Trace(string s) {

            // Console.WriteLine(s);
            // Console.ReadKey();
        }
        static void Main(string[] args)
        {
            var embedFiles = new List<string>() {
                /*[FILES]*/ // <== Do not touch this tag
            };

            var executionDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"));

            Trace(string.Format("Extracting in folder {0}", executionDir));
            
            foreach(var embedFile in embedFiles) {

                var fileName = DynamicSugar.Embed.Resources.SaveBinaryResourceAsFile(Assembly.GetExecutingAssembly(), executionDir, embedFile);
                Trace(fileName);
            }
            // [ARGUMENTS] will be replace by the right value during the process of creating the self extractor
            var arguments = @"[ARGUMENTS]".Replace("{Path}", executionDir).Replace("{DQ}", "\"");

            var exe = "[EXE]";
            if(!String.IsNullOrEmpty(exe)) {
                
                Trace("[EXE] "+arguments); // [EXE] will be replace by the right value during the process of creating the self extractor
                Process.Start(exe, arguments); // Should we wait and clean the extracted file ?
            }
        }
    }
}

/*
 
csc.exe /debug /define:DEBUG /target:exe /out:VrmAgentInstaller.exe /resource:VrmAgentInstaller.msi SelfExtractor.cs
csc.exe /target:exe /out:VrmAgentInstaller.exe /resource:VrmAgentInstaller.msi SelfExtractor.cs
 
 */