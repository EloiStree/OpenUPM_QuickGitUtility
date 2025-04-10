using System.Collections.Generic;
using UnityEngine;

namespace Eloi.Git
{
    public class GitMono_GitsInProjectsAsText : MonoBehaviour {

        public bool m_useGitCloneInFrontOf = true;
        private void Reset()
        {
            Refresh();
        }
        public string m_unityAssetPath;
    public List<GitLinkOnDisk> m_inParentsPath;
    public List<GitLinkOnDisk> m_inTheProject;
    public string m_gitAsTextList;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        string assetPath = UnityPaths.GetUnityAssetsPath();
        QuickGit.GetGitsInParents(assetPath, out m_inParentsPath);
        QuickGit.GetGitsInDirectory(assetPath, out m_inTheProject, true);
            m_gitAsTextList = "```\n";
            foreach (var item in m_inTheProject)
                if (m_useGitCloneInFrontOf)
                    m_gitAsTextList +="git clone "+ item.m_gitLink + "\n";
                else 
                    m_gitAsTextList += item.m_gitLink + "\n";

            m_gitAsTextList += "```\n";


        }

    }



}