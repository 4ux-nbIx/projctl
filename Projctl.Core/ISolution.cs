﻿namespace Projctl.Core
{
    #region Namespace Imports

    using System.Collections.Generic;

    #endregion


    public interface ISolution
    {
        string FileName { get; }
        string FullPath { get; }
        string DirectoryPath { get; }
        IEnumerable<IProject> GetProjects(bool recursive = false);
    }
}