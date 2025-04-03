using System.Collections.Generic;
using UnityEngine;

namespace Eloi.Git
{
    public class GitMono_GitsInProjectsAsText : MonoBehaviour {

    public string m_unityAssetPath;
    public List<GitLinkOnDisk> m_inParentsPath;
    public List<GitLinkOnDisk> m_inTheProject;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        string assetPath = UnityPaths.GetUnityAssetsPath();
        QuickGit.GetGitsInParents(assetPath, out m_inParentsPath);
        QuickGit.GetGitsInDirectory(assetPath, out m_inTheProject, true);
    }

}



}