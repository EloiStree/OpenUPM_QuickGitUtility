using System.Collections.Generic;
using UnityEngine;

namespace Eloi.Git
{
    public class GitMono_IsGitInAssetsFolder : MonoBehaviour
    {

        public string m_gitPathToLookFor = "https://github.com/USER/PROJECT.git";
        private void Reset()
        {
            Refresh();
        }
        public List<GitLinkOnDisk> m_inTheProject;

        public bool m_useOnValidate = true;
        public bool m_isGitInAssetsFolder;
        public GitLinkOnDisk m_gitLinkOnDisk;

        public void OnValidate()
        {
            if (!m_useOnValidate)
                return;
            Refresh();
        }
        [ContextMenu("Refresh")]
        public void Refresh()
        {
            string assetPath = UnityPaths.GetUnityAssetsPath();
            QuickGit.GetGitsInDirectory(assetPath, out m_inTheProject, true);
            m_isGitInAssetsFolder = false;
            m_gitLinkOnDisk = null;
            foreach (var item in m_inTheProject)
            {
                if (item.m_gitLink == null || item.m_gitLink.Trim().Length <= 0)
                    continue;
                if (item.m_gitLink.Trim().ToLower().IndexOf(m_gitPathToLookFor.Trim().ToLower())>=0)
                {
                    m_isGitInAssetsFolder = true;
                    m_gitLinkOnDisk = item;
                    break;
                }
            }
        }

    }

}