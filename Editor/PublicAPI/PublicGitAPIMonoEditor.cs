using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PublicGitAPIMonoEditor : EditorWindow
{

    public Info m_info = new Info();
    [SerializeField]
    public class Info
    {
        public string m_userNameGitLab = "";
        public List<JsonGitLabProject> m_gitlabProjects;
        public string m_userNameGithub = "";
        public List<JsonGitHubProject> m_githubProjects;
        public string m_listOfClonable="";
        public Vector2 m_scrollInfo= new Vector2();
    }

    [MenuItem("ꬲ🧰/ Git Utility/ Git Public Lab&Hub List")]
    static void Init()
    {
        PublicGitAPIMonoEditor window = (PublicGitAPIMonoEditor)EditorWindow.GetWindow(typeof(PublicGitAPIMonoEditor));
        window.titleContent = new GUIContent("Gits Public Project List");
        window.LoadInfo();
        window.Show();
    }
    void OnGUI()
    {
        if (m_info == null)
            LoadInfo();
        if (m_info == null)
            m_info = new Info();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GitHub");
        m_info.m_userNameGithub = EditorGUILayout.TextField(m_info.m_userNameGithub);
        EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GitLab");
        m_info.m_userNameGitLab = EditorGUILayout.TextField(m_info.m_userNameGitLab);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Download from servers"))
        {
            m_info.m_gitlabProjects = PublicGitLab.GetProjects(m_info.m_userNameGitLab, 5);
            m_info.m_githubProjects = PublicGitHub.GetProjects(m_info.m_userNameGithub, 5);

            RefreshCloneList();

        }
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Sort by name"))
        {
         m_info.m_gitlabProjects =
             m_info.m_gitlabProjects.
             OrderBy(k => k.http_url_to_repo).ToList();

         m_info.m_githubProjects =
            m_info.m_githubProjects.
            OrderBy(k => k.clone_url).ToList();

            RefreshCloneList();


        }
        if (GUILayout.Button("Sort by created"))
        {
            m_info.m_gitlabProjects =
             m_info.m_gitlabProjects.
             OrderBy(k => k.created_at).ToList();

            m_info.m_githubProjects =
               m_info.m_githubProjects.
               OrderBy(k => k.created_at).ToList();

            RefreshCloneList();


        }
        EditorGUILayout.EndHorizontal();

        m_info.m_scrollInfo=EditorGUILayout.BeginScrollView(m_info.m_scrollInfo);
        EditorGUILayout.TextArea(m_info.m_listOfClonable);
        EditorGUILayout.EndScrollView();

    }

    private void RefreshCloneList()
    {
        m_info.m_listOfClonable =
            string.Join("\n",
            m_info.m_gitlabProjects.
            Select(k => k.http_url_to_repo).ToList());
        m_info.m_listOfClonable += "\n";
        m_info.m_listOfClonable +=
                        string.Join("\n",
                        m_info.m_githubProjects.
                        Select(k => k.clone_url).ToList());
    }

    private void OnDestroy()
    {
        SaveInfo();
    }
    public void LoadInfo()
    {

        string m_infoSave = PlayerPrefs.GetString("GitLabHubLists");
        m_info = JsonUtility.FromJson<Info>(m_infoSave);

    }
    public void SaveInfo()
    {

        string  m_infoSave = JsonUtility.ToJson(m_info);
        PlayerPrefs.SetString("GitLabHubLists", m_infoSave);
    }


}
