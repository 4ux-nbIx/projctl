namespace Projctl.Test
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    using Projctl.Core;

    #endregion


    [TestClass]
    public class CreateSolutionFilterTest : TestBase
    {
        [DataTestMethod]
        [DataRow(ProjectA,                      ProjectA, ProjectB, ProjectC, ProjectD)]
        [DataRow("**\\*.csproj",                ProjectA, ProjectB, ProjectC, ProjectD)]
        [DataRow(ProjectB,                      ProjectB, ProjectD)]
        [DataRow("TestSolution\\**\\*B.csproj", ProjectB, ProjectD)]
        public async Task ShouldCreateSolutionFilter(string projectGlob, params string[] results)
        {
            await InvokeAsync($"create-solution-filter {projectGlob} --solution {TestSolution}");

            OutString.Should().NotBeNullOrWhiteSpace();

            var solutionFilterFile = JsonConvert.DeserializeObject<SolutionFilterFile>(OutString);

            Verify(results, solutionFilterFile, Directory.GetCurrentDirectory());
        }

        [DataTestMethod]
        [DataRow(ProjectA,                      ProjectA, ProjectB, ProjectC, ProjectD)]
        [DataRow("**\\*.csproj",                ProjectA, ProjectB, ProjectC, ProjectD)]
        [DataRow(ProjectB,                      ProjectB, ProjectD)]
        [DataRow("TestSolution\\**\\*B.csproj", ProjectB, ProjectD)]
        public async Task ShouldCreateSolutionFilterFile(string projectGlob, params string[] results)
        {
            var fileName = Path.Combine(TestSolutionDirectory, "Test.slnf");
            await InvokeAsync($"create-solution-filter {projectGlob} --solution {TestSolution} --destination {fileName}");

            var json = await File.ReadAllTextAsync(fileName);
            var solutionFilterFile = JsonConvert.DeserializeObject<SolutionFilterFile>(json);

            Verify(results, solutionFilterFile, Path.GetDirectoryName(fileName));
        }

        private static void Verify(IReadOnlyCollection<string> results, SolutionFilterFile solutionFilterFile, string directory)
        {
            solutionFilterFile.SolutionFilter.Should().NotBeNull();

            solutionFilterFile.SolutionFilter.Path.Should().Be(Path.GetRelativePath(directory, TestSolution));

            var solutionDirectoryName = Path.GetDirectoryName(TestSolution);

            solutionFilterFile.SolutionFilter.Projects.Should()
                .HaveCount(results.Count)
                .And.Contain(results.Select(p => Path.GetRelativePath(solutionDirectoryName!, p)));
        }
    }
}