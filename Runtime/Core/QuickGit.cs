using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public static class QuickGit
{

    public static void RemoveAllEmptyFolders(string wherePath)
    {
        RemoveGitKeepInFolders(wherePath);
        string [] paths = GetAllFolders(wherePath, true);
        foreach (var path in paths)
        {
            if (Directory.Exists(path)) {
                string[] files = Directory.GetFiles(path);
                if (files.Length == 0) { 
                    Directory.Delete(path,true);
                    if (File.Exists(path + ".meta"))
                        File.Delete(path + ".meta");
                }
            }
        }
        CreateGitKeepInEmptyFolders(wherePath);
    }
    public static void CreateGitKeepInEmptyFolders(string wherePath, string keepTextContent = "Hey mon ami, tu aimes ça manger des patates ?")
    {
        string[] paths = GetAllFolders(wherePath, true);
        foreach (var path in paths)
        {
            if (Directory.Exists(path) && Directory.GetFiles(path).Length == 0)
            {
                File.WriteAllText(path + "/.gitkeep", keepTextContent);
            }
        }
    }

    public static void RemoveGitKeepInFolders(string wherePath)
    {
        string[] paths = GetAllFolders(wherePath, true);
        foreach (var path in paths)
        {
            if (Directory.Exists(path))
            {
                if (File.Exists(path + "/.gitkeep"))
                    File.Delete(path + "/.gitkeep");
            }
        }
    }
    public static void RefreshGitKeepInEmptyFolder(string wherePath, string keepTextContent = "Hey mon ami, tu aimes ça manger des patates ?")
    {
        RemoveGitKeepInFolders(wherePath);
        CreateGitKeepInEmptyFolders(wherePath);
    }

    public static void AddFileInEmptyFolder(string folderPath)
    {
        List<string> folders = GetAllFolders(folderPath, true).ToList();
        FindRemoveFilesIn(ref folders, ".git");
        for (int i = 0; i < folders.Count; i++)
        {
            string path = folders[i];
            string[] files = Directory.GetFiles(path);
            string emptyPath = path + "\\empty.txt";
            string emptyPathMeta = path + "\\empty.txt.meta";
            files = Remove(files, emptyPathMeta, emptyPath);
            //            UnityEngine.Debug.Log("<> " + path + " : " + files.Length);

            bool isEmpty = files.Length <= 0;

            if (isEmpty)
            {
                File.WriteAllText(emptyPath, "Avoid empty folder");
            }
            else
            {
                if (File.Exists(emptyPath))
                    File.Delete(emptyPath);
                if (File.Exists(emptyPathMeta))
                    File.Delete(emptyPathMeta);
            }
        }
    }

    public static void GetGitInDirectory(string path, out GitLinkOnDisk gitLink, bool searchInChildens)
    {
        List<GitLinkOnDisk> gits;
        GetGitsInDirectory(path, out gits, searchInChildens);
        if (gits.Count > 0)
            gitLink = gits[0];
        else gitLink = null;

    }

    private static string[] Remove(string[] files, string emptyPathMeta, string emptyPath)
    {
        List<string> f = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] == emptyPath || files[i] == emptyPathMeta)
            { }
            else {
                f.Add(files[i]);
            }

        }
        return f.ToArray();
    }

    public static void GetGitsInDirectory(string directoryPath, out List<GitLinkOnDisk> found, bool withChildrensFolders = true)
    {
        found = GetGitsInGivenDirectories(GetAllFolders(directoryPath, withChildrensFolders, withChildrensFolders));
    }
    public static void GetGitsInDirectory(string directoryPath, out List<GitLinkOnDisk> found,bool useRootFolder , bool withChildrensFolders = true)
    {
        found = GetGitsInGivenDirectories(GetAllFolders(directoryPath, useRootFolder, withChildrensFolders));
    }


    public static void GetGitsFromLeaf(List<GitLinkOnDisk> givenGits, out GitLinkOnDisk gitOnTopOfPath)
    {
        if (givenGits.Count <=0) gitOnTopOfPath = null;
        else
        gitOnTopOfPath = givenGits.OrderByDescending(k => k.GetDirectoryPath().Length).First();
    }
    public static void GetGitsOnFromRoot(List<GitLinkOnDisk> givenGits, out GitLinkOnDisk gitOnTopOfPath)
    {
        if (givenGits.Count <= 0) gitOnTopOfPath = null;
        else
            gitOnTopOfPath = givenGits.OrderBy(k => k.GetDirectoryPath().Length).First();
    }
    public enum PathReadDirection { RootToLeaf, LeafToRoot}
    public static void GetGitInParents(string path, PathReadDirection readMode, out GitLinkOnDisk git) {
        List<GitLinkOnDisk> links;
        GetGitsInParents(path, out links);
        if (readMode == PathReadDirection.RootToLeaf)
            GetGitsOnFromRoot(links, out git);
        else 
            GetGitsFromLeaf(links, out git);
    }
    public static void GetGitsInParents(string path, out List<GitLinkOnDisk> links)
    {
        links = new List<GitLinkOnDisk>();
        string[] parentsPath = UnityPaths.GetAllParents(path, true);
        for (int i = 0; i < parentsPath.Length; i++)
        {
            if (IsPathContaintGitRoot(parentsPath[i]))
            {
                GitLinkOnDisk gd = new GitLinkOnDisk(parentsPath[i]);
                links.Add(gd);
            }
        }
    }

    private static bool IsPathContaintGitRoot(object path)
    {
        return Directory.Exists(path + "/.git");
    }

    public static bool IsPathInAssetFolder(string currentPath)
    {
        currentPath = currentPath.Replace("\\", "/");
        string path = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Assets";
        return currentPath.IndexOf(path) > -1;
    }
    public static bool IsPathOutsideOfAssetFolder(string currentPath)
    {
        return !IsPathInAssetFolder(currentPath);
    }
    public static bool IsPathInProjectFolder(string currentPath)
    {
        currentPath = currentPath.Replace("\\", "/");
        string path = Directory.GetCurrentDirectory().Replace("\\", "/") ;
        return currentPath.IndexOf(path) > -1;

    }

   

    public static bool IsPathOutsideOfProjectFolder(string currentPath)
    {
        return !IsPathInProjectFolder(currentPath);
    }
    public static bool IsGitInsideProject(string currentPath)
    {
        return !IsPathOutsideOfAssetFolder(currentPath);
    }

    public static List<GitLinkOnDisk> GetGitsInGivenDirectories(string[] directoriesPath)
    {
        List<GitLinkOnDisk> packages = new List<GitLinkOnDisk>();
        for (int i = directoriesPath.Length-1; i >= 0; i--)
        {

            string p = directoriesPath[i];
            bool isGitFolder = IsPathHasGitRootFolder(p);
            if (isGitFolder)
                packages.Add(new GitLinkOnDisk(p));
        }
        return packages;
    }


    public static bool IsPathHasGitRootFolder(string directoryPath) {
        return Directory.Exists(directoryPath + "/.git");
    }

    public static bool IsPathIsGitRootFormat(string directoryPath)
    {if (directoryPath.Length < 6) return false;

        return directoryPath.ToLower().IndexOf("/.git") == directoryPath.Length - 5
                ||
                directoryPath.ToLower().IndexOf("\\.git") == directoryPath.Length - 5;
    }

    public static bool IsGitFolderWihtUrl(string directoryPath)
    {
        if (!IsPathHasGitRootFolder(directoryPath))
            return false;
        string url = "";
        GetGitUrl(directoryPath,out url);
        return string.IsNullOrWhiteSpace(url);
    }

    public static bool IsFolderEmpty(string whereGitIs)
    {
        if (!Directory.Exists(whereGitIs))
            return false;
        return Directory.GetFiles(whereGitIs).Length <= 0;
    }

   

    private static void FindRemoveFilesIn(ref List<string> folders, string toFound)
    {
        for (int i = folders.Count - 1; i >=0; i--)
        {
            if (folders[i].IndexOf(toFound) > -1)
                folders.RemoveAt(i);
        }
    }

   
  

    public static string [] GetAllFolders(string folderPath, bool containGivenFolder, bool withChildren=true) {
        if (string.IsNullOrEmpty(folderPath.Trim())) 
            return new string[0];

        List<string> pathList = Directory.GetDirectories(folderPath, "*", withChildren? SearchOption.AllDirectories: SearchOption.TopDirectoryOnly).ToList();
        if (containGivenFolder)
            pathList.Add(folderPath);
        return pathList.ToArray();
    }

    

    public static void OpenCmd(string gitDirectoryPath)
    {
        if (gitDirectoryPath.Length < 2) return;

        char disk = 'C';
        if (gitDirectoryPath[1] == ':')
            disk = gitDirectoryPath[0];
        //string cmd = disk + ":" + "&" + "cd \"" + gitDirectoryPath + "\"";

        string strCmdText;
        strCmdText = "/K " + disk + ":" + " && cd " + gitDirectoryPath + " && git status";
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = strCmdText;
        process.Start();
    }
    public static void Clone(string gitUrl, string gitDirectoryPath)
    {
        WindowCMD.RunCommands(new string[] {
                "git clone "+ gitUrl+ " "+ gitDirectoryPath
          }, gitDirectoryPath);
    }

    public static void Pull(string gitDirectoryPath)
    {
        WindowCMD.RunCommands(new string[] {
                "git pull"
          }, gitDirectoryPath);
    }
    public static void Add(string gitDirectoryPath)
    {
        WindowCMD.RunCommands(new string[] {
                "git add -A"
          }, gitDirectoryPath);
    }
    public static void Commit(string gitDirectoryPath, string commitDescription = "")
    {
        if (string.IsNullOrWhiteSpace(commitDescription))
            commitDescription = GetTime();
        WindowCMD.RunCommands(new string[] {
                "git commit -m \"Save: " + commitDescription + "\""
          }, gitDirectoryPath);
    }

    private static string GetTime()
    {
        return DateTime.Now.ToString("MM\\/dd\\/yyyy h\\:mm tt");
    }

    public static void Push(string gitDirectoryPath)
    {
        WindowCMD.RunCommands(new string[] {
                "git push"
          }, gitDirectoryPath);
    }
    public static bool m_debugState = false;
    public static void  SetDebugOn(bool useDebug)
    {
        m_debugState = useDebug;
    }
    public static bool GetDebugState() { return m_debugState; }
    public static void AddCommitPull(string gitDirectoryPath, string commitDescription = "")
    {
        if (string.IsNullOrWhiteSpace(commitDescription))
            commitDescription = GetTime();
        WindowCMD.RunCommands(new string[] {
                "git add -A",
                "git commit -m \"Save: " + commitDescription + "\"",
                "git pull"
          }, gitDirectoryPath);
    }
    public static void PullPushWithAddAndCommit(string gitDirectoryPath, string commitDescription = "")
    {
        if (string.IsNullOrWhiteSpace(commitDescription))
            commitDescription = GetTime();
        WindowCMD.RunCommands(new string[] {
                "git add -A",
                "git commit -m \"Save: " + commitDescription + "\"",
                "git pull",
                "git add -A",
                "git commit -m \"Merge: "+ commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }
    public static void AddCommitAndPush(string gitDirectoryPath, string commitDescription = "")
    {
        if (string.IsNullOrWhiteSpace(commitDescription))
            commitDescription = GetTime();
        WindowCMD.RunCommands(new string[] {
                "git add -A",
                "git commit -m \"" + commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }


    public static void CreateLocal(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(directoryPath + "/test.md", "Test");
        WindowCMD.RunCommands(new string[] {
                "git init .",
                "git add .",
                "git commit -m \"First commit\"",
          }, directoryPath);
        //$ git push -u origin master

    }
    public static void PushLocalToNewOnline(GitServer server, string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {
        gitCreatedUrl = "";
        switch (server)
        {
            //case GitServer.GitHub:
            //    PushLocalToGitHub(directoryPath, userName, newRepoName, out gitCreatedUrl);
            //    break; 
            case GitServer.GitLab:
                PushLocalToGitLab(directoryPath, userName, newRepoName, out gitCreatedUrl);
                break;
            case GitServer.GitHub:
                PushLocalToGitHub(directoryPath, userName, newRepoName, out gitCreatedUrl);
                break;
            default:
                break;
        }
    }
    public static void PushLocalToGitHub(string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {
       throw new NotImplementedException("Impossible or not in my skills contact me if youknow how to do it. I tried for hours.");
    //    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(newRepoName))
    //        gitCreatedUrl = "https://github.com/" + userName + "/" + newRepoName + ".git";
    //    else
    //        gitCreatedUrl = "";
    //    //https://kbroman.org/github_tutorial/pages/init.html
    //    RunCommands(new string[] {
    //                "git add .",
    //                "git commit -m \"Local to Remote\"",

    ////                "git remote add origin git@github.com:"+userName+"/"+newRepoName+".git",
    //                "git remote add origin https://github.com/"+userName+"/"+newRepoName+"",
    //                "git push --set-upstream https://github.com/"+userName+"/"+newRepoName+".git master",
    //                "git push -u origin master"
    //          }, directoryPath);
    }
    public static void PushLocalToGitLab(string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {


        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(newRepoName))
            gitCreatedUrl = "https://gitlab.com/" + userName + "/" + newRepoName + ".git";
        else
            gitCreatedUrl = "";

        //https://docs.gitlab.com/ee/gitlab-basics/create-project.html
        //git push --set-upstream https://gitlab.example.com/namespace/nonexistent-project.git master
        //git push --set-upstream address/your-project.git
        WindowCMD.RunCommands(new string[] {
                "git add .",
                "git commit -m \"Local to Remote\"",
                "git push --set-upstream https://gitlab.com/"+userName+"/"+newRepoName+".git master",
                "git push -u origin master"
          }, directoryPath);

    }
    public static void RemoveFolder(string directoryPath)
    {
        RemoveFiles(directoryPath);

        WindowCMD.RunCommands(new string[] {
                "del /S /F /AH "+directoryPath,
                "rmdir "+directoryPath
          }, directoryPath);
    }
    public static void RemoveFiles( string directoryPath)
    {
        string[] pathfiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        string[] pathfilesOwn = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        for (int i = 0; i < pathfilesOwn.Length; i++)
        {
            pathfilesOwn[i] = "takeown / A / F" + pathfilesOwn[i];
        }
        for (int i = 0; i < pathfiles.Length; i++)
        {

            pathfiles[i] = "del /F /AH " + pathfiles[i];
        }
        List<string> files = new List<string>();
        files.AddRange(pathfiles);
        files.AddRange(pathfilesOwn);
        WindowCMD.RunCommands(files.ToArray(), directoryPath);
    }
   
    public static bool GetGitUrl(string rootDirectoryPath, out string url)
    {
        url = "";
        if (string.IsNullOrEmpty(rootDirectoryPath))
            return false;
        string filePath = rootDirectoryPath + "/.git/config";
        if (!File.Exists(filePath))
            return false;

        string[] lines = File.ReadAllLines(filePath);
        string gitUrl = "";
        for (int i = 0; i < lines.Length; i++)
        {
            int urlIndex = lines[i].IndexOf("url =");
            if (urlIndex >= 0)
            {
                gitUrl = lines[i].Substring(urlIndex + "url =".Length).Trim();
                break;
            }

        }
        if (gitUrl == "")
        {
            for (int i = 0; i < lines.Length; i++)
            {
                int urlIndex = lines[i].IndexOf("remote =");
                if (urlIndex >= 0)
                {
                    gitUrl = lines[i].Substring(urlIndex + "remote =".Length).Trim();
                    break;
                }
            }

        }

        url = gitUrl;
        return !string.IsNullOrEmpty(url);
    }
    public enum GitServer { GitHub, GitLab }
}
[System.Serializable]
public class GitLink
{
    public string m_gitLink;
    public bool IsLinkDefined() { return !string.IsNullOrWhiteSpace(m_gitLink); }
}
[System.Serializable]
public class GitLinkOnDisk : GitLink
{
    public string m_projectDirectoryPath;

    public GitLinkOnDisk(string directoryPath)
    {
        QuickGit.GetGitUrl(directoryPath,out m_gitLink);
        this.m_projectDirectoryPath = directoryPath;
    }

    public void OpenFolder()
    {
        if (Directory.Exists(m_projectDirectoryPath))
            Application.OpenURL(m_projectDirectoryPath);
    }
    public void OpenHost()
    {
            Application.OpenURL(m_gitLink);
    }
    public bool IsPathDefined() { return !string.IsNullOrWhiteSpace(m_projectDirectoryPath); }

    public string GetDirectoryPath()
    {
        return m_projectDirectoryPath;
    }

    public bool Exist()
    {
        return Directory.Exists(m_projectDirectoryPath) && Directory.Exists(m_projectDirectoryPath+"/.git");
    }

    public string GetUrl()
    {
        return m_gitLink;
    }

    public string GetName()
    {
        int indexOf = m_gitLink.LastIndexOf("/");
        if (indexOf < 0)
            indexOf = m_gitLink.LastIndexOf("\\");
        if (indexOf < 0)
            indexOf = 0;
        return m_gitLink.Substring(indexOf).Replace(".git", "")
            .Replace("/", "").Replace("\\", "");
    }
}


