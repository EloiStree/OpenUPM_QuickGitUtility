using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class UnityPaths 
{


    public static string GoUpInPath(string currentPath)
    {
        int lastIndex = currentPath.LastIndexOf('/');
        if (lastIndex < 0)
            lastIndex = currentPath.LastIndexOf('\\');
        if (lastIndex < 0)
            return "";
        return currentPath.Substring(0, lastIndex);
    }

    public static string StartByPoint( string text)
    {
        if (text.Length <= 0)
            return ".";
        if (text[0] != '.')
            return "." + text;
        return text;
    }

    public static string[] GetAllParents(string path, bool addGivenPath)
    {
        List<string> result = new List<string>();
        if (addGivenPath)
            result.Add(path);
        bool hasFinish=false;
        do {
            path = GoUpInPath(path);
            hasFinish = path.Length <= 0;
            if (!hasFinish)
                result.Add(path);
        }
        while (!hasFinish);
        return result.ToArray();
    }

    public static void GetPathOf(string whereToLook,string fileName, string[] extensions, out bool foundOne, out string filePathFound)
    {
        foundOne = false;
        filePathFound = "";
        for (int i = 0; i < extensions.Length; i++)
        {
            string path = whereToLook + "/" + fileName;
            extensions[i] = StartByPoint( extensions[i]);
            path += extensions[i];
            if (File.Exists(path))
            {
                foundOne = true;
                filePathFound = path;
            }
        }

    }

    public static string[] Filter(string[] files, string[] notAuthorizedExtentsion)
    {
        List<string> authorizedPath = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            for (int j = 0; j < notAuthorizedExtentsion.Length; j++)
            {
                if(files[i].EndsWith(notAuthorizedExtentsion[j]))
                   authorizedPath.Add(files[i]);
            }
        }
        return authorizedPath.ToArray();
    }

    public static string GetUnityAssetsPath()
    {
        return Directory.GetCurrentDirectory()+"/Assets";
    }
    public static string GetUnityRootPath()
    {
        return Directory.GetCurrentDirectory();
    }

    public static string ReplaceByBackslash(string path)
    {
        return path.Replace("\\", "/");
    }
    public static string ReplaceBySlash(string path)
    {
        return path.Replace("/","\\");
    }

    public static Regex alphaNumSpaceRegex = new Regex("[^a-zA-Z0-9 ]");
    public static Regex namespaceRegex = new Regex("[^a-zA-Z0-9\\.]");
    public static Regex alphaNumRegex = new Regex("[^a-zA-Z0-9]");
    public static string AlphaNumeric(string text, bool allowSpace = false)
    {
        if (allowSpace)
            return alphaNumSpaceRegex.Replace(text, "");
        return alphaNumRegex.Replace(text, "");
    }

    public static string NamespaceTrim(string text)
    {
       return namespaceRegex.Replace(text, "");
    }

    public static string GetLastPartOfPath(string path)
    {
        int lastIndex = path.LastIndexOf('/');
        if (lastIndex < 0)
            lastIndex = path.LastIndexOf('\\');
        if (lastIndex < 0)
            return path;
        return path.Substring(lastIndex+1 );
    }

    public static string GetRelativePathInAssets(string containedPath)
    {
        return GetRelativePath(GetUnityAssetsPath(), containedPath);
    }
    public static string GetRelativePathInProjectRoot(string containedPath)
    {
        return GetRelativePath(GetUnityRootPath(), containedPath);
    }
    public static string GetRelativePath(string containerPath, string containedPath) {
        string rp = ReplaceBySlash(containedPath).Replace(ReplaceBySlash(containerPath), "");
        if (rp.Length > 0 && (rp[0] == '/' || rp[0] == '\\'))
            rp = rp.Substring(1);

        return rp;
    }

}
