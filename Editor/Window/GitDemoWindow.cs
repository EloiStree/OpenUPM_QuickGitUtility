using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

public class GitDemoWindow : EditorWindow
{
    public static string m_infoSave="";
    public Info m_info = new Info();
    public GitInTheUnityProject m_gitInProject = new GitInTheUnityProject();
    [SerializeField]
    public class Info {

        public string m_directoryRelative = "";
        public string m_directoryAbsolute = "";

        public string[] m_gitLinksPath= new string[0];
        public List<GitLinkOnDisk> m_gitLinks = new List<GitLinkOnDisk>();
        public bool m_displayAllFolder;
        public bool m_displayAllGitInFolder=true;
        public string m_projectGitInParent="";

        public bool[] m_displayInfoInGits = new bool[100];
        public bool[] m_displayprojectsInfo = new bool[100];
    }

    


    [MenuItem("ꬲ🧰/ Git Utility/ Prototype / Selection")]
    static void Init()
    {
        GitDemoWindow window = (GitDemoWindow)EditorWindow.GetWindow(typeof(GitDemoWindow));
        window.titleContent = new GUIContent("Gits Selection");
        window.LoadInfo();
        window.Show();
    }
    private void OnDestroy()
    {
        SaveInfo();
    }

    public void LoadInfo()
    {

        m_infoSave = PlayerPrefs.GetString("GitDemoWindow");
        m_info = JsonUtility.FromJson<Info>(m_infoSave);

    }
    public void SaveInfo() {

        m_infoSave = JsonUtility.ToJson(m_info);
        PlayerPrefs.SetString("GitDemoWindow", m_infoSave);
    }

    void OnGUI()
    {
        if (m_info == null)
            m_info = new Info();

        bool foundPath = false;
        string previousPath = m_info.m_directoryRelative;
        UnityPathSelectionInfo selected;
        UnityPathSelectionInfo.Get(out foundPath, out selected);
        m_info.m_directoryRelative = selected.GetRelativePath(true);
        m_info.m_directoryAbsolute = selected.GetAbsolutePath(true);
         bool changeFound = m_info.m_directoryRelative != previousPath;

        bool resquestRefresh = GUILayout.Button("Refresh");
        if (resquestRefresh || changeFound)
        {
            m_gitInProject.Refresh();
            m_info.m_gitLinksPath = QuickGit.GetAllFolders(m_info.m_directoryAbsolute, true);
            QuickGit.GetGitsInDirectory(m_info.m_directoryAbsolute, out m_info.m_gitLinks);
            m_info.m_projectGitInParent = ""; 
            GitLinkOnDisk gd;
            QuickGit.GetGitInParents(selected.GetAbsolutePath(true), QuickGit.PathReadDirection.LeafToRoot, out gd);
            if (gd != null) m_info.m_projectGitInParent = gd.GetDirectoryPath() ;



        }
       

        EditorGUILayout.TextField("Relative >", m_info.m_directoryRelative);
        EditorGUILayout.TextField("Abosluete > ", m_info.m_directoryAbsolute);

       // if (foundPath) 
        {
            if(!string.IsNullOrEmpty(m_info.m_projectGitInParent))
                EditorGUILayout.TextField("Parent Git", m_info.m_projectGitInParent.Length<=0?"Project is not in a git.": m_info.m_projectGitInParent);

            m_info.m_displayAllFolder= EditorGUILayout.Toggle("All Directory", m_info.m_displayAllFolder);
            if (m_info.m_displayAllFolder) { 
                for (int i = 0; i < m_info.m_gitLinksPath.Length; i++)
                {
                    m_info.m_gitLinksPath[i] = EditorGUILayout.TextField("Directory", m_info.m_gitLinksPath[i]);

                 }
            }
          
            m_info.m_displayAllGitInFolder = EditorGUILayout.Toggle("All Repository", m_info.m_displayAllGitInFolder);
            if (m_info.m_displayAllGitInFolder) { 
                for (int i = 0; i < m_info.m_gitLinks.Count; i++)
                {
                    GitEditorDrawer.DisplayGitInfoWithCommand(m_info.m_gitLinks[i],ref m_info.m_displayprojectsInfo[i], ref  m_info.m_displayInfoInGits[i]);
                    EditorGUILayout.Space();
                }
            }
        }

        GUILayout.BeginHorizontal();
        GitEditorDrawer.ButtonDeleteEmptyFiles(m_info.m_directoryAbsolute);
        GitEditorDrawer.ButtonRefreshGitKeeper(m_info.m_directoryAbsolute);
        GUILayout.EndHorizontal();

    }

    

}