using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GitInTheUnityProject
{
    public List<GitLinkOnDisk> m_inParentsPath;
    public List<GitLinkOnDisk> m_inTheProject;

    public GitInTheUnityProject() {
        Refresh();
    }

    public void Refresh()
    {
        string assetPath = UnityPaths.GetUnityAssetsPath();
        QuickGit.GetGitsInParents(assetPath, out m_inParentsPath);
        QuickGit.GetGitsInDirectory(assetPath, out m_inTheProject, true);
    }

    public List<GitLinkOnDisk> GetParents(bool refresh=false) { return m_inParentsPath; }
    public List<GitLinkOnDisk> GetGitInProject(bool refresh = false) { return m_inTheProject; }

    public void AutoSaveLocal(string commitMessage="")
    {
        for (int i = 0; i < m_inTheProject.Count; i++)
        {

            QuickGit.Add(m_inTheProject[i].GetDirectoryPath());
            QuickGit.Commit(m_inTheProject[i].GetDirectoryPath(), commitMessage);
        }
    }

    public void AutoSaveAndPush(string commitMessage = "")
    {
        for (int i = 0; i < m_inTheProject.Count; i++)
        {
            QuickGit.PullPushWithAddAndCommit(m_inTheProject[i].GetDirectoryPath(), commitMessage);
        }
    }
}