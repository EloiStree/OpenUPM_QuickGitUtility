namespace Eloi.Git
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class GitMono_FindFileWithUnresolvedConflict : MonoBehaviour {


        public string m_folderPath;

        public string[] m_filesFoundInFolder;
        public List<string> m_filesFoundWithConflict = new List<string>();


        [ContextMenu("Scan files")]
        public void ScanFiles() { 
        
            m_filesFoundInFolder = Directory.GetFiles(m_folderPath,"*",SearchOption.AllDirectories);

            m_filesFoundWithConflict.Clear();
            bool fileHasConflict =false;
            foreach (string file in m_filesFoundInFolder) { 
            
                string text = File.ReadAllText(file);
                string[] lines = text.Split(new char[] { '\n' });
                bool gitStart=false, gitMiddle = false, gitEnd = false;
                fileHasConflict = false;
                foreach (string line in lines) {

                    if (!gitStart && line.StartsWith("<<<<<<< "))
                    {
                        gitStart = true;
                    }
                    else if (!gitMiddle && line.StartsWith("======="))
                    {
                        gitMiddle = true;
                    }
                    else if (!gitEnd && line.StartsWith(">>>>>>> "))
                    {
                        gitEnd = true;
                    }
                    if (gitStart && gitMiddle && gitEnd) {

                        m_filesFoundWithConflict.Add(file);
                        fileHasConflict =true;
                    }
                    if (fileHasConflict) break;
                }
            }
        }
    }

}