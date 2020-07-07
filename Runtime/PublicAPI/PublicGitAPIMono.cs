using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PublicGitAPIMono : MonoBehaviour
{
    public string m_userNameGitLab = "EloiStree";
    public List<JsonGitLabProject> m_gitlabProjects;
    public string m_userNameGithub = "EloiStree";
    public List<JsonGitHubProject> m_githubProjects;
    public string m_debug;
    public int m_maxPages=5;
    [ContextMenu("Load all projects GitLab")]
    void LoadAllProjectsList_GitLab()
    {
        m_gitlabProjects = PublicGitLab.GetProjects(m_userNameGitLab, m_maxPages);
    }
    [ContextMenu("Load all projects GitHub")]
    void LoadAllProjectsList_GitHub()
    {

        m_githubProjects = PublicGitHub.GetProjects(m_userNameGithub, m_maxPages);
        m_githubProjects = m_githubProjects.OrderByDescending(k => k.GetPushedDate()).ToList();
        m_debug = m_githubProjects[0].GetPushedDate().ToString("yyyy-MM-dd HH mm ss");
    }



}
