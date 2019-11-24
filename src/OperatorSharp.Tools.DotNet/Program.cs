using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OperatorSharp.Tools.DotNet
{
    [Command("dotnet-operatorsharp"), Subcommand(typeof(GenerateCrdsCommand), typeof(BuildCrdsCommand))]
    class Program
    {
        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify at a subcommand.");
            app.ShowHelp();
            return 1;
        }
    }
}
