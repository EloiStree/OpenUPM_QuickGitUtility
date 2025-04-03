using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Eloi.Git
{

    public class AddGitCommentMenuToProjectEditor : MonoBehaviour
    {

        static void RunCommand(string cmd)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe", "/K " + cmd)
                {
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };
            process.Start();
            process.WaitForExit();
        }




        [MenuItem("Assets/ꬲ🧰/Git/Specific/Add as save directory")]
        private static void AddToSaveDirectory()
        {
            string where = GetSelectedDirectoryPath();

            if (where.Length > 0)
            {
                QuickGit.GetGitInParents(where, QuickGit.PathReadDirection.LeafToRoot, out GitLinkOnDisk git);
                UnityEngine.Debug.Log(git.m_projectDirectoryPath);
                AddToGitQuickSaveDirectory(git.m_projectDirectoryPath);

            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Add>Commit>Pull>Push>Server")]
        private static void AddCommitPullPushServer()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.GetGitInParents(where, QuickGit.PathReadDirection.LeafToRoot, out GitLinkOnDisk git);
                QuickGit.PullPushWithAddAndCommit(git.GetDirectoryPath());
                Application.OpenURL(git.GetUrl());
            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Clipboard/Clone from clipboard")]
        private static void GitCloneFromClipboard()
        {
            string url = GUIUtility.systemCopyBuffer.Trim();
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                UnityEngine.Debug.Log("Test");
                QuickGit.Clone(url, where);
            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Clipboard/Get Git Url")]
        private static void GitGetUrlInClipboard()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.GetGitInParents(where, QuickGit.PathReadDirection.LeafToRoot, out GitLinkOnDisk git);
                GUIUtility.systemCopyBuffer = git.GetUrl();
            }
        }



        [MenuItem("Assets/ꬲ🧰/Git/Add>Commit>Pull>Push")]
        private static void AddCommitPullPush()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.GetGitInParents(where, QuickGit.PathReadDirection.LeafToRoot, out GitLinkOnDisk git);
                QuickGit.PullPushWithAddAndCommit(git.GetDirectoryPath());
            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Debug Log Git Found", priority = 1)]
        private static void GetFoundDebugLog()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.GetGitInParents(where, QuickGit.PathReadDirection.LeafToRoot, out GitLinkOnDisk git);
                UnityEngine.Debug.Log(git.m_projectDirectoryPath);
            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Open CMD")]
        private static void OpenStatus()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.OpenCmd(where);
            }
        }
        [MenuItem("Assets/ꬲ🧰/Git/Open Server")]
        private static void OpenServer()
        {
            string where = GetSelectedDirectoryPath();
            if (where.Length > 0)
            {
                QuickGit.GetGitUrl(where, out string url);
                Application.OpenURL(url);
            }
        }

        public static string GetSelectedDirectoryPath()
        {
            string p = GetSelectedFilePathMethod();
            if (File.Exists(p))
            {
                return Path.GetDirectoryName(p);
            }
            if (Directory.Exists(p))
            {
                return p;
            }
            return "";
        }
        private static string GetSelectedFilePathMethod()
        {
            // Get the selected object(s) in the Project window
            UnityEngine.Object[] selectedObjects = Selection.objects;

            foreach (UnityEngine.Object obj in selectedObjects)
            {
                // Get the path of the selected object
                string assetPath = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(assetPath))
                {
                    // Convert the asset path to an absolute path
                    string absolutePath = Path.GetFullPath(assetPath);
                    return absolutePath;
                }

            }
            return "";
        }


        public static void AddToGitQuickSaveDirectory(string path)
        {
            // The git command to execute
            string arguments = "/K git config --global --add safe.directory \"" + path + "\"";

            // Create a new process to run the git command
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processStartInfo))
                {
                    // Read the output (if any)
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Log the output
                    if (!string.IsNullOrEmpty(output.Trim()))
                    {
                        UnityEngine.Debug.Log("Git Command Output: " + output);
                    }

                    // Log any errors
                    if (!string.IsNullOrEmpty(error.Trim()))
                    {
                        UnityEngine.Debug.LogError("Git Command Error: " + error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to execute git command: " + ex.Message);
            }

        }
    }

}