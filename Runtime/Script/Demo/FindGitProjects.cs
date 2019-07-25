using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGitProjects : MonoBehaviour
{
    public string m_directory;
    public string[] m_gitLinksPath;
    public List<GitLinkOnDisk > m_gitLinks;

    private void Start()
    {
        if (string.IsNullOrEmpty(m_directory))
            return;
        m_gitLinksPath = QuickGit.GetAllFolders(m_directory,true);
        m_gitLinks = QuickGit.GetGitProjectsInDirectory(m_directory);
    }
}
