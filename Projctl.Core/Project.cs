namespace Projctl.Core
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using JetBrains.Annotations;

    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Globbing;

    #endregion


    public class Project : IProject
    {
        [NotNull]
        private readonly Microsoft.Build.Evaluation.Project _project;

        private readonly IProjectFactory _projectFactory;

        [CanBeNull]
        private List<IProject> _referencedProjects;

        public Project(IProjectFactory projectFactory, Microsoft.Build.Evaluation.Project msbuildProject)
        {
            _project = msbuildProject;
            _projectFactory = projectFactory;
        }


        public string DirectoryPath => _project?.DirectoryPath ?? Path.GetDirectoryName(FullPath);
        public string FullPath => _project.FullPath;
        public bool IsDirty => _project.IsDirty;
        public string Name => Path.GetFileNameWithoutExtension(FullPath);

        public bool ContainsItems(CompositeGlob items, string[] projectItemTypes = null)
        {
            IList<(string Type, string Path)> projectItems = _project.Items.Select(x => (x.ItemType, x.GetFullPath())).ToList();
            projectItems.Add(("ProjectFile", _project.FullPath));

            if (projectItemTypes != null && projectItemTypes.Length > 0)
            {
                projectItems = projectItems.Where(i => projectItemTypes.Contains(i.Type)).ToList();
            }

            return projectItems.Any(i => items.IsMatch(i.Path));
        }

        public ICollection<ProjectItem> GetItems(string itemType) => _project.GetItems(itemType);

        public ProjectProperty GetProperty(string name) => _project.GetProperty(name);

        public IEnumerable<IProject> GetReferencedProjects(bool recursive = false)
        {
            var projects = LoadProjectReferences().ToList();

            return !recursive ? projects : projects.Concat(projects.SelectMany(p => p.GetReferencedProjects(true))).Distinct();
        }

        public IEnumerable<IProject> LoadProjectReferences()
        {
            if (_referencedProjects != null)
            {
                return _referencedProjects;
            }

            _referencedProjects = _project.GetProjectReferences()
                .Select(p => _projectFactory.LoadProject(Path.GetFullPath(Path.Combine(DirectoryPath, p.EvaluatedInclude))))
                .Where(p => p != null)
                .Distinct()
                .ToList();

            return _referencedProjects;
        }
    }
}