namespace Projctl.Test
{
    #region Namespace Imports

    using System;
    using System.CommandLine.IO;
    using System.CommandLine.Parsing;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Build.Locator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion


    [TestClass]
    public class TestBase
    {
        protected const string ProjectA = "TestSolution\\ProjectA\\ProjectA.csproj";

        protected const string ProjectAClass1Path = "TestSolution\\ProjectA\\Class1.cs";
        protected const string ProjectAClass2Path = "TestSolution\\ProjectA\\Class2.cs";
        protected const string ProjectB = "TestSolution\\ProjectB\\ProjectB.csproj";
        protected const string ProjectC = "TestSolution\\ProjectC\\ProjectC.csproj";
        protected const string ProjectD = "TestSolution\\ProjectD\\ProjectD.csproj";

        protected const string TestSolution = "TestSolution\\Test.sln";
        protected const string TestSolutionDirectory = "TestSolution\\";

        private TestConsole _console;
        private Parser _parser;

        protected string Error => _console.Error.ToString();
        protected string[] Out => _console.Out.ToString()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        protected string OutString => _console.Out.ToString();

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Directory.SetCurrentDirectory("TestCodebase");

            MSBuildLocator.RegisterDefaults();
        }

        [TestInitialize]
        public void Init()
        {
            _console = new TestConsole();
            _parser = new Parser(Program.BuildCli());
        }

        protected async Task InvokeAsync(string commandLine)
        {
            await _parser.InvokeAsync(commandLine, _console);
        }
    }
}