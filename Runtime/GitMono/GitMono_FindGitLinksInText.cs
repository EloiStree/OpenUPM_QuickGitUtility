using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Eloi.Git
{
    public class GitMono_FindGitLinksInText : MonoBehaviour
    {
        [TextArea(0, 10)]
        public string m_textToParse;
        [TextArea(0, 10)]
        public string m_gitLinks;
        public bool m_addCloneCommand = true;
        public string m_regexPattern = @"(https?:\/\/[^\s]+\.git(?:#(?:commit|tag)\/[^\s]+)?)";

        [ContextMenu("Parse")]
        public void Parse()
        {

            m_gitLinks = "";
            Regex regex = new Regex(m_regexPattern);
            MatchCollection matches = regex.Matches(m_textToParse);
            string[] matchesAsLine = matches.Select(m => m.Value.Trim  ()).OrderBy(k=>k).Distinct().ToArray();

            foreach (string match in matchesAsLine)
            {
                if (m_addCloneCommand)
                {
                    m_gitLinks += "git clone " + match + "\n\r";
                }
                else
                {
                    m_gitLinks += match + "\n\r";
                }
            }

        }
    }

}