using System.Linq;
using System.Text.RegularExpressions;

namespace Eloi.Git
{
    using System.Collections.Generic;
    using UnityEngine;


    public class GitMono_RemoveAllDotGitInFolder: MonoBehaviour
    {
        public string m_folderPath;
        public RemoveType m_removeType = RemoveType.Delete;
        public enum RemoveType { Delete, RenameAsGitI}


        public string[] directories;
        public List<string> folderToRemove;
        [ContextMenu("DoTheThing")]
        public void DoTheThing()
        {
            directories= System.IO.Directory.GetDirectories(m_folderPath, "*", System.IO.SearchOption.AllDirectories);
            folderToRemove= new List<string>();
            foreach (string directory in directories)
            {
                // if the folder parent is /.git 
                if (directory.EndsWith(".git"))
                {
                    folderToRemove.Add(directory);
                }
             
            }
            folderToRemove = folderToRemove.Distinct().ToList();

            foreach (string folder in folderToRemove)
            {
                try
                {
                    if (m_removeType == RemoveType.Delete)
                    {
                        System.IO.Directory.Delete(folder, true);
                    }
                    else
                    {
                        string newName = folder.Replace(".git", ".giti");
                        System.IO.Directory.Move(folder, newName);
                        System.IO.Directory.Delete(folder, true);

                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error removing folder: " + folder + "\n" + e.Message);
                }
            }
        }
    }

}