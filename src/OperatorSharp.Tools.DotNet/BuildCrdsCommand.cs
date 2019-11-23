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

        private async Task<int> OnExecuteAsync(IConsole console)
        {
            try
            {
                // Find project files
                if (string.IsNullOrEmpty(ProjectFile) || !File.Exists(ProjectFile))
                {
                    ProjectFile = FindProjectFile();
                }

                await WriteTargets(ProjectFile);

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
            var process = Process.Start(psi);
            process.WaitForExit();
        }

        private async Task WriteTargets(string projectFile)
        {
            string targetsContent = await LoadFileFromEmbeddedResource(targetsMoniker);
            var targetFileName = Path.GetFileName(projectFile) + "." + targetsMoniker;
            var projectExtPath = Path.Combine(Path.GetDirectoryName(projectFile), "obj");
            var targetFile = Path.Combine(projectExtPath, targetFileName);

            await File.WriteAllTextAsync(targetFile, targetsContent);
        }

        private string FindProjectFile()
        {
            var projectFiles = Directory.EnumerateFiles( Directory.GetCurrentDirectory(), "*.*proj")
                .Where(f => !f.EndsWith(".xproj"));

            if (!projectFiles.Any()) { throw new FileNotFoundException("Can't find any projects"); }
            if (projectFiles.Skip(1).Any()) { throw new FileNotFoundException("Multiple project files found. Specify which project to use with the --project option"); }

            return projectFiles.First();
        }

        private async Task<string> LoadFileFromEmbeddedResource(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream($"OperatorSharp.Tools.DotNet.{resourcePath}"))
            using (StreamReader rdr = new StreamReader(stream))
            {
                return await rdr.ReadToEndAsync();
            }
        }
    }
}
