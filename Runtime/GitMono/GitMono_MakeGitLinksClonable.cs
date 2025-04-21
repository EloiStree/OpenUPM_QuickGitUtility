using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eloi.Git
{
    public class GitMono_MakeGitLinksClonable : MonoBehaviour 
    {
        [TextArea(0,10)]
        public string m_gitLinks;
        public List<string> m_gitLinksList;
        [TextArea(0, 10)]
        public string m_gitLinksClonable;

        [ContextMenu("Do The Thing")]
        public void DoTheThing() {


            m_gitLinksList = m_gitLinks.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            m_gitLinksClonable = "";
            foreach (string link in m_gitLinksList)
            {
                if (string.IsNullOrEmpty(link))
                    continue;
                if (!link.Contains(".git"))
                    continue;
                string [] splitBySlash = link.Split('/');
                if (splitBySlash.Length < 2)
                    continue;
                string gitDirectory = string.Join("/", splitBySlash[splitBySlash.Length - 2], splitBySlash[splitBySlash.Length - 1].Split("#").First().Replace(".git",""));
                string clone = "git clone " + link.Trim() + " " + gitDirectory.Trim() + "\n\r";
                m_gitLinksClonable += clone;

            }
        }



    }

}