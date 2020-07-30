using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

public class GitInProjectWindow : EditorWindow
{
    public static string m_infoSave;
    public Info m_info = new Info(50);
    public GitInTheUnityProject m_gitInProject = new GitInTheUnityProject();
    [SerializeField]
    public class Info
    {
        public int m_tableCapacity=10;
        public bool[] m_displayInfoInGitsInProject   = new bool[10];
        public bool[] m_displayprojectsInfoInProject = new bool[10];
        public bool[] m_displayInfoInGitsParent      = new bool[10];
        public bool[] m_displayprojectsInfoParent    = new bool[10];
        public bool m_inProject=true;
        public bool m_inParent=true;
        public Vector2 m_scollrPackagePosition = new Vector2();

        public Info(int maxSizeOfTableDirtyCode) {
            m_tableCapacity = maxSizeOfTableDirtyCode;
            m_displayInfoInGitsInProject = new bool[maxSizeOfTableDirtyCode];
            m_displayprojectsInfoInProject = new bool[maxSizeOfTableDirtyCode];
            m_displayInfoInGitsParent = new bool[maxSizeOfTableDirtyCode];
            m_displayprojectsInfoParent = new bool[maxSizeOfTableDirtyCode];
            for (int i = 0; i < maxSizeOfTableDirtyCode; i++)
            {
                m_displayInfoInGitsInProject[i]  = false;
                m_displayprojectsInfoInProject[i]= false;
                m_displayInfoInGitsParent[i]     = true;
                m_displayprojectsInfoParent[i]   = true;



            }
        }
    }

    


    [MenuItem("ꬲ🧰/ Git Utility / View All")]
    static void Init()
    {
        GitInProjectWindow window = (GitInProjectWindow)EditorWindow.GetWindow(typeof(GitInProjectWindow));
        window.titleContent =new GUIContent( "Gits View All");
        window.LoadInfo();
        window.Show(); 
    }
    private void OnDestroy()
    {
        SaveInfo();
    }

    public void LoadInfo()
    {

        m_infoSave = PlayerPrefs.GetString("GitsProjectWindow");
        m_info = JsonUtility.FromJson<Info>(m_infoSave);

    }
    public void SaveInfo() {

        m_infoSave = JsonUtility.ToJson(m_info);
        PlayerPrefs.SetString("GitsProjectWindow", m_infoSave);
    }


    public int space=10;
    void OnGUI()
    {
        if (m_info == null)
            m_info = new Info(50);

        m_info.m_scollrPackagePosition = GUILayout.BeginScrollView(m_info.m_scollrPackagePosition);
        if (m_info == null)
            m_info = new Info(50);
        if(GUILayout.Button("Refresh"))
            m_gitInProject.Refresh();
        GUILayout.BeginHorizontal();
        GitEditorDrawer.ButtonDeleteEmptyFiles(UnityPaths.GetUnityAssetsPath());
        GitEditorDrawer.ButtonRefreshGitKeeper(UnityPaths.GetUnityAssetsPath());
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save Them All (Add>Commit)"))
            m_gitInProject.AutoSaveLocal();
        if (GUILayout.Button("Push Them All (A>C>Pull>Push)"))
            m_gitInProject.AutoSaveAndPush();
        m_info.m_inParent = EditorGUILayout.Toggle(" > Git(s) In Parent", m_info.m_inParent, EditorStyles.boldLabel);
        if (m_info.m_inParent)
        {
            
            List<GitLinkOnDisk> gits = m_gitInProject.GetParents();

            if (gits.Count <= 0)
                GUILayout.Label("None found");
            for (int i = 0; i < gits.Count; i++)
            {
                if (i >= m_info.m_tableCapacity) break;
                GitEditorDrawer.DisplayGitInfoWithCommand(gits[i], ref m_info.m_displayprojectsInfoParent[i], ref m_info.m_displayInfoInGitsParent[i]);
                EditorGUILayout.Space();
            }

        }
        GUILayout.Space(space);
        m_info.m_inProject = EditorGUILayout.Toggle(" > Git(s) In Assets", m_info.m_inProject, EditorStyles.boldLabel);
        if (m_info.m_inProject)
        {
            List<GitLinkOnDisk> gits = m_gitInProject.GetGitInProject();
            if (gits.Count <= 0)
                GUILayout.Label("None found");
            for (int i = 0; i < gits.Count; i++)
            {
                if (i >= m_info.m_tableCapacity) break;
                GitEditorDrawer.DisplayGitInfoWithCommand(gits[i], ref m_info.m_displayprojectsInfoInProject[i], ref m_info.m_displayInfoInGitsInProject[i]);
                EditorGUILayout.Space();
            }
        }

        GUILayout.Space(space);
        GUILayout.EndScrollView();

    }

    

}