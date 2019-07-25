using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public static class QuickGit
{

    //public static void RemoveAllEmptyFolders()
    //{
    //}
    public static void AddFileInEmptyFolder(string folderPath)
    {
        List<string> folders = GetAllFolders(folderPath,true).ToList();
        FindRemoveFilesIn(ref folders,".git");
        for (int i = 0; i < folders.Count; i++)
        {
            string path = folders[i];
            string[] files= Directory.GetFiles(path);
            string emptyPath = path + "\\empty.txt";
            string emptyPathMeta = path + "\\empty.txt.meta";
            files=Remove( files, emptyPathMeta, emptyPath);
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

    private static string[] Remove( string[] files, string emptyPathMeta, string emptyPath)
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

    public static List<GitLinkOnDisk> GetGitProjectsInDirectory(string directoryPath)
    {
        return GetGitProjectsInDirectory( GetAllFolders(directoryPath, true) );
    }
    public static List<GitLinkOnDisk> GetGitProjectsInDirectory(string[] directoriesPath)
    {
        List<GitLinkOnDisk> packages = new List<GitLinkOnDisk>();
        for (int i = directoriesPath.Length-1; i >= 0; i--)
        {
            string p = directoriesPath[i];
            bool isGitFolder = IsFolderContainGitProject(p);
            if (isGitFolder)
                packages.Add(new GitLinkOnDisk(p));
        }
        return packages;
    }
   
    private static bool IsFolderContainGitProject(string p)
    {
        string[] directories = Directory.GetDirectories(p);
        for (int i = 0; i < directories.Length; i++)
        {
            
            if (directories[i].IndexOf("/.git/") == directories[i].Length - 6
                || directories[i].IndexOf("\\.git\\") == directories[i].Length - 6
                || directories[i].IndexOf("/.git") == directories[i].Length - 5
                || directories[i].IndexOf("\\.git") == directories[i].Length - 5)
            {

                return true;
            }


        }
        return false;
    }
    


    private static void FindRemoveFilesIn(ref List<string> folders, string toFound)
    {
        for (int i = folders.Count - 1; i >=0; i--)
        {
            if (folders[i].IndexOf(toFound) > -1)
                folders.RemoveAt(i);
        }
    }

    public static string [] GetAllFolders(string folderPath, bool containGivenFolder) {
        List<string> pathList = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories).ToList();
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
        RunCommands(new string[] {
                "git clone "+ gitUrl+ " "+ gitDirectoryPath
          }, gitDirectoryPath);
    }
    public static void Pull(string gitDirectoryPath)
    {
        RunCommands(new string[] {
                "git pull"
          }, gitDirectoryPath);
    }

    public static bool m_debugState = false;
    public static void  SetDebugOn(bool useDebug)
    {
        m_debugState = useDebug;
    }
    public static bool GetDebugState() { return m_debugState; }

    public static void PullAddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"Save: " + commitDescription + "\"",
                "git pull",
                "git add .",
                "git commit -m \"Merge: "+ commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }
    public static void AddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"" + commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }

    static void RunCommands(string[] cmds, string workingDirectory)
    {
        if (workingDirectory.Length < 2) return;

        char disk = 'C';
        if (workingDirectory[1] == ':')
            disk = workingDirectory[0];

        var process = new Process();
        var psi = new ProcessStartInfo();
        psi.FileName = "cmd.exe";
        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.WorkingDirectory = workingDirectory;
        process.StartInfo = psi;
        process.Start();
        process.OutputDataReceived += (sender, e) => {
            if(GetDebugState())
                UnityEngine.Debug.Log(e.Data); };
        process.ErrorDataReceived += (sender, e) => {
            if (GetDebugState())
                UnityEngine.Debug.Log(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();



        using (StreamWriter sw = process.StandardInput)
        {
            sw.WriteLine(disk + ":");
            sw.WriteLine("cd " + workingDirectory);
            foreach (var cmd in cmds)
            {
                if (GetDebugState())
                    UnityEngine.Debug.Log("> " + cmd);
                sw.WriteLine(cmd);
            }
        }
        process.WaitForExit();
    }

    public static void CreateLocal(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(directoryPath + "/test.md", "Test");
        RunCommands(new string[] {
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
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"Local to Remote\"",
                "git push --set-upstream https://gitlab.com/"+userName+"/"+newRepoName+".git master",
                "git push -u origin master"
          }, directoryPath);

    }
    public static void RemoveFolder(string directoryPath)
    {
        RemoveFiles(directoryPath);

        RunCommands(new string[] {
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
        RunCommands(files.ToArray(), directoryPath);
    }
   
    public static bool GetGitUrl(string directoryPath, out string url)
    {
        url = "";
        string filePath = directoryPath + "/.git/config";
        string[] lines = File.ReadAllLines(filePath);
        string gitUrl = "";
        for (int i = 0; i < lines.Length; i++)
        {
            int urlIndex = lines[i].IndexOf("url =");
            if (urlIndex >= 0)
            {
                gitUrl = lines[i].Substring(urlIndex + 5).Trim();
                break;
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
    
    public void OpenFolder() {
        if(Directory.Exists(m_projectDirectoryPath))
          Application.OpenURL(m_projectDirectoryPath);
    }
    public bool IsPathDefined() { return !string.IsNullOrWhiteSpace(m_projectDirectoryPath); }

}


