using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GitEditorDrawer
{


    public static void DisplayGitInfoWithCommand(GitLinkOnDisk gitDirectory, ref bool displayInfo, ref bool displayAdvance)
    {
        if (displayInfo = EditorGUILayout.Foldout(displayInfo, "Git: " + gitDirectory.GetName() ))
        {

            DisplayGitLink(gitDirectory);
            if (displayAdvance = EditorGUILayout.Foldout(displayAdvance, "  Git Commands"))
            {
                    DisplayGitCommands(gitDirectory);
            }
        }
    }


    public static void DisplayGitCommands(GitLinkOnDisk gitDirectory)
    {
        bool hasUrl = gitDirectory.HasUrl();

        if (gitDirectory.Exist())
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add -a"))
            {
                QuickGit.Add(gitDirectory.GetDirectoryPath());
            }
            if (GUILayout.Button("Commit"))
            {
                QuickGit.Commit(gitDirectory.GetDirectoryPath());
            }
            if (hasUrl && GUILayout.Button("Pull"))
            {
                QuickGit.Pull(gitDirectory.GetDirectoryPath());
            }
            if (hasUrl && GUILayout.Button("Push"))
            {
                QuickGit.Push(gitDirectory.GetDirectoryPath());
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (hasUrl && GUILayout.Button("Add>Commit>Pull"))
            {
                QuickGit.AddCommitAndPush(gitDirectory.GetDirectoryPath());
            }
            if (hasUrl && GUILayout.Button("A>C>Pull + A>C>push"))
            {
                QuickGit.PullPushWithAddAndCommit(gitDirectory.GetDirectoryPath());
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open explorer"))
            {
                Application.OpenURL(gitDirectory.GetDirectoryPath());
            }
            if (GUILayout.Button("See Status"))
            {
                QuickGit.OpenCmd(gitDirectory.GetDirectoryPath());
            }
            if (hasUrl && GUILayout.Button("Go to Server"))
            {
                Application.OpenURL(gitDirectory.GetUrl());
            }

            GUILayout.EndHorizontal();

        }



    }

    public static void DisplayGitLink(GitLinkOnDisk git)
    {
        GUIStyle button = new GUIStyle(GUI.skin.button);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("> Git:", button, GUILayout.MaxWidth(50)))
        {
            Application.OpenURL(git.GetUrl());
        }
        EditorGUILayout.TextField(git.GetUrl());
        if (GUILayout.Button("> Path:", new GUIStyle(GUI.skin.button), GUILayout.MaxWidth(50)))
        {
            Application.OpenURL(git.GetDirectoryPath());
        }
        EditorGUILayout.TextField(git.GetDirectoryPath());
        GUILayout.EndHorizontal();
    }

    public static void ButtonDeleteEmptyFiles(string absolutPath)
    {
        if (GUILayout.Button("Delete Empty folder in directory"))
        {
            QuickGit.RemoveAllEmptyFolders(absolutPath);
            AssetDatabase.Refresh();
        }
    }
    public static void ButtonRefreshGitKeeper(string absolutPath)
    {
        if (GUILayout.Button("Refresh gitkeep in directory"))
        {
            QuickGit.RefreshGitKeepInEmptyFolder(absolutPath);
            AssetDatabase.Refresh();
        }
    }
}
