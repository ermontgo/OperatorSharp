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

        [Option("-k|--kubernetes-version", CommandOptionType.SingleValue, Description = "The version of the Kubernetes CRD API to target. This must be either \"v1\" or \"v1beta1\".")]
        public string Version { get; set; }

        protected Dictionary<string, ICustomResourceDefinitionBuilder> builders = new Dictionary<string, ICustomResourceDefinitionBuilder>()
        {
            { "v1beta1", new V1beta1CustomResourceDefinitionBuilder() },
            { "v1", new V1CustomResourceDefinitionBuilder() },
            { "", new V1beta1CustomResourceDefinitionBuilder() }
        };

        // Unsure how the CommandLineUtils API works and if it requires OnExecute to be private, so adding a public shim for unit testing
        public int Execute(IConsole console) => OnExecute(console);

        private int OnExecute(IConsole console)
        {
            console.WriteLine("Generating CRDs");
            if (!File.Exists(AssemblyPath)) { throw new FileNotFoundException("Could not find the provided assembly", AssemblyPath); }
            var directory = Directory.CreateDirectory(OutputFolder);

            var version = (Version ?? string.Empty).ToLower().Trim();
            if (!builders.ContainsKey(version))
            {
                throw new ArgumentOutOfRangeException("kubernetes-version", "The kubernetes version specified is not available. Please choose \"v1\" or \"v1beta1\".");
            }

            var assembly = Assembly.LoadFrom(AssemblyPath);

            console.WriteLine($"Located assembly {assembly.FullName}");

            var searcher = new CustomResourceSearcher();
            var builder = builders[version];
            var customResourceTypes = searcher.FindCustomResourceTypes(assembly);

            foreach (var customResourceType in customResourceTypes)
            {
                try
                {
                    console.WriteLine($"Generating CRD for {customResourceType.Name}");
                    var crd = builder.BuildDefinition(customResourceType);
                    var crdYaml = crd.ToYaml();
                    var crdPath = Path.Combine(directory.FullName, $"{crd.Name}.yaml");

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
