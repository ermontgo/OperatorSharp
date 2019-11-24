using k8s;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OperatorSharp.Tools.DotNet
{
    [Command("generate-crds", Description = "Generates CRDs from a provided assembly into a provided output folder")]
    public class GenerateCrdsCommand
    {
        [Option("-a|--assembly",CommandOptionType.SingleValue)]
        public string AssemblyPath { get; set; }

        [Option("-o|--output", CommandOptionType.SingleValue)]
        public string OutputFolder { get; set; }

        // Unsure how the CommandLineUtils API works and if it requires OnExecute to be private, so adding a public shim for unit testing
        public int Execute(IConsole console) => OnExecute(console);

        private int OnExecute(IConsole console)
        {
            console.WriteLine("Generating CRDs");
            if (!File.Exists(AssemblyPath)) { throw new FileNotFoundException("Could not find the provided assembly", AssemblyPath); }
            var directory = Directory.CreateDirectory(OutputFolder);

            var assembly = Assembly.LoadFrom(AssemblyPath);

            console.WriteLine($"Located assembly {assembly.FullName}");

            var searcher = new CustomResourceSearcher();
            var builder = new CustomResourceDefinitionBuilder();
            var customResourceTypes = searcher.FindCustomResourceTypes(assembly);

            foreach (var customResourceType in customResourceTypes)
            {
                try
                {
                    console.WriteLine($"Generating CRD for {customResourceType.Name}");
                    var crd = builder.BuildDefinition(customResourceType);
                    var crdYaml = Yaml.SaveToString(crd);
                    var crdPath = Path.Combine(directory.FullName, $"{crd.Metadata.Name}.yaml");

                    console.WriteLine($"Writing CRD yaml to {crdPath}");
                    File.WriteAllText(crdPath, crdYaml);
                }
                catch (MissingMetadataAttributeException mmaEx)
                {
                    console.Error.WriteLine($"{mmaEx.Message}");
                    console.Error.WriteLine("Skipping...");
                }
                catch (Exception ex)
                {
                    console.Error.WriteLine(ex.ToString());
                }
            }

            return 0;
        }
    }
}
