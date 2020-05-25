using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GitUtilityEditor : MonoBehaviour
{

    public static bool IsFile(string path) { return File.Exists(path); }
    public static bool IsDirectory(string path) { return Directory.Exists(path); }
    public static void GetSelectedPathInUnity(out bool found, out UnityPathSelectionInfo info)
    {
        UnityPathSelectionInfo.Get(out found, out info);
    }

    
}
