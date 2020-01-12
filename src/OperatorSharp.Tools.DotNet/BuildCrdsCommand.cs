using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OperatorSharp.Tools.DotNet
{
    [Command("build-crds", Description = "Builds the project and generates CRD yaml files for the custom resources in the project")]
    class BuildCrdsCommand
    {
        const string targetsMoniker = "operatorsharp.g.targets";

        [Option("-p|--project")]
        public string ProjectFile { get; set; }

        [Option("-o|--output")]
        public string OutputPath { get; set; }

        [Option("-k|--kubernetes-version", CommandOptionType.SingleValue, Description = "The version of the Kubernetes CRD API to target. This must be either \"v1\" or \"v1beta1\".")]
        public string Version { get; set; }

        private int OnExecute(IConsole console)
        {
            try
            {
                // Find project files
                if (string.IsNullOrEmpty(ProjectFile) || !File.Exists(ProjectFile))
                {
                    ProjectFile = FindProjectFile();
                }

                WriteTargets(ProjectFile);

                StartBuildProcess(ProjectFile);

                return 0;
            }
            catch (Exception ex)
            {
                console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private void StartBuildProcess(string projectFile)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"msbuild \"{projectFile}\" /t:_OperatorSharpGenerateCRDs /nologo"
            };

            if (!string.IsNullOrEmpty(OutputPath))
            {
                psi.Arguments += $" /p:CrdOutputPath=\"{OutputPath}\"";
            }

            if (!string.IsNullOrEmpty(Version))
            {
                psi.Arguments += $" /p:CrdVersion=\"{Version}\"";
            }

            var process = Process.Start(psi);
            process.WaitForExit();
        }

        private void WriteTargets(string projectFile)
        {
            string targetsContent = LoadFileFromEmbeddedResource(targetsMoniker);
            var targetFileName = Path.GetFileName(projectFile) + "." + targetsMoniker;
            var projectExtPath = Path.Combine(Path.GetDirectoryName(projectFile), "obj");
            var targetFile = Path.Combine(projectExtPath, targetFileName);

            File.WriteAllText(targetFile, targetsContent);
        }

        private string FindProjectFile()
        {
            var projectFiles = Directory.EnumerateFiles( Directory.GetCurrentDirectory(), "*.*proj")
                .Where(f => !f.EndsWith(".xproj"));

            if (!projectFiles.Any()) { throw new FileNotFoundException("Can't find any projects"); }
            if (projectFiles.Skip(1).Any()) { throw new FileNotFoundException("Multiple project files found. Specify which project to use with the --project option"); }

            return projectFiles.First();
        }

        private string LoadFileFromEmbeddedResource(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream($"OperatorSharp.Tools.DotNet.{resourcePath}"))
            using (StreamReader rdr = new StreamReader(stream))
            {
                return rdr.ReadToEnd();
            }
        }
    }
}
