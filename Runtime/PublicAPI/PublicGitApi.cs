using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

public class PublicGitHub {

    //users/EloiStree/repos?page=1&per_page=100

    public static string m_gitLinkAPI = "https://api.github.com/";


    public static List<JsonGitHubProject> GetProjects(string exactUserId, int pagesToLoad = 3)
    {

        List<JsonGitHubProject> result = new List<JsonGitHubProject>();
        for (int i = 0; i < pagesToLoad; i++)
        {
            bool wasLoaded = false;
            string url = m_gitLinkAPI + string.Format("users/{0}/repos?page={1}&per_page=100", exactUserId, i + 1);
            //https://api.github.com/users/EloiStree/repos?page=1&per_page=100
            string errorMessage;
            string page = DownloadWebPage.DownloadPageWithWebClient(url);
            // page= page.Trim().Substring(1, page.Length-2);
            page = " { \"values\": " + page + " } ";
           // Debug.Log("URL" + wasLoaded + "/" + errorMessage + ":" + url);
            Debug.Log("URL:" + url);
            Debug.Log("Page:" + page);
            JsonListGitHubProjects pl = JsonUtility.FromJson<JsonListGitHubProjects>(page);
            //Debug.Log(pl.values. Length);
            result.AddRange(pl.values);

        }
        return result;
    }
}


public class PublicGitLab 
{
    public static string m_gitLinkAPI="https://gitlab.com/api/v4/";


    public static List<JsonGitLabProject> GetProjects(string exactUserId,int pagesToLoad = 3) {

        List<JsonGitLabProject> result = new List<JsonGitLabProject>();
        for (int i = 0; i < pagesToLoad; i++)
        {
            bool wasLoaded = false;
            string url = m_gitLinkAPI + string.Format("users/{0}/projects?order_by=id&page={1}&per_page=100", exactUserId, i+1);
            //https://gitlab.com/api/v4/users/EloiStree/projects?order_by=id&page=1&per_page=100
            string error;
            string page = DownloadWebPage.DownloadPageWithSuccess(url, out wasLoaded, out error);
           // page= page.Trim().Substring(1, page.Length-2);
            page = " { \"values\": " + page + " } ";
            Debug.Log("Page:" + page);
            JsonListGitLabProjects pl = JsonUtility.FromJson<JsonListGitLabProjects>(page);
           // Debug.Log(pl.values. Length);
            result.AddRange(pl.values);

        }
        return result;
    }

    public static DateTime ConvertToDate(string dateAsString) {
        //2020-04-29T18:45:15Z
        return DateTime.ParseExact(dateAsString, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture);
        

    }
}

public  class OnlineGitProject {

    public string m_name;
    public string m_description;
    public string m_defaultBranch;
    public string m_cloneLink;
    public DateTime m_createdDate;
    public DateTime m_updateDate;


    public  string GetName() { return m_name; }
    public  string GetDescription() { return m_description; }
    public  string GetDefaultBranchName() { return m_defaultBranch; }
    public  string GetCloneLink() { return m_cloneLink; }
    public  DateTime GetCreatedDate() { return m_createdDate; }
    public  DateTime GetUpdatedDate() { return m_updateDate; }

}


[System.Serializable]
public class JsonListGitHubProjects
{
    public JsonGitHubProject[] values;
}
[System.Serializable]
public class JsonGitHubProject
{
    //"name": "2012_04_29_DesignPatternInUnity
    public string name;
    //"id": 260010577,
    public string id;
    //"node_id": "MDEwOlJlcG9zaXRvcnkyNjAwMTA1Nzc=",
    public string node_id;
    //"full_name": "EloiStree/2012_04_29_DesignPatternInUnity",
    //"private": false,
    //"owner": {
    //  "login": "EloiStree",
    //  "id": 20149493,
    //  "node_id": "MDQ6VXNlcjIwMTQ5NDkz",
    //  "avatar_url": "https://avatars1.githubusercontent.com/u/20149493?v=4",
    //  "gravatar_id": "",
    //  "url": "https://api.github.com/users/EloiStree",
    //  "html_url": "https://github.com/EloiStree",
    //  "followers_url": "https://api.github.com/users/EloiStree/followers",
    //  "following_url": "https://api.github.com/users/EloiStree/following{/other_user}",
    //  "gists_url": "https://api.github.com/users/EloiStree/gists{/gist_id}",
    //  "starred_url": "https://api.github.com/users/EloiStree/starred{/owner}{/repo}",
    //  "subscriptions_url": "https://api.github.com/users/EloiStree/subscriptions",
    //  "organizations_url": "https://api.github.com/users/EloiStree/orgs",
    //  "repos_url": "https://api.github.com/users/EloiStree/repos",
    //  "events_url": "https://api.github.com/users/EloiStree/events{/privacy}",
    //  "received_events_url": "https://api.github.com/users/EloiStree/received_events",
    //  "type": "User",
    //  "site_admin": false
    //},
    //"html_url": "https://github.com/EloiStree/2012_04_29_DesignPatternInUnity",
    public string html_url;
    //"description": "Just a cheat-sheet for me to practice and remember of how we can use design pattern in Unity ",
    public string description;
    //"fork": false,   
    public bool fork;
    //"url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity",
    //"forks_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/forks",
    public string forks_url;
    //"keys_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/keys{/key_id}",
    //"collaborators_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/collaborators{/collaborator}",
    //"teams_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/teams",
    //"hooks_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/hooks",
    //"issue_events_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/issues/events{/number}",
    //"events_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/events",
    //"assignees_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/assignees{/user}",
    //"branches_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/branches{/branch}",
    //"tags_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/tags",
    //"blobs_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/git/blobs{/sha}",
    //"git_tags_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/git/tags{/sha}",
    //"git_refs_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/git/refs{/sha}",
    //"trees_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/git/trees{/sha}",
    //"statuses_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/statuses/{sha}",
    //"languages_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/languages",
    //"stargazers_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/stargazers",
    //"contributors_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/contributors",
    //"subscribers_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/subscribers",
    //"subscription_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/subscription",
    //"commits_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/commits{/sha}",
    //"git_commits_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/git/commits{/sha}",
    //"comments_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/comments{/number}",
    //"issue_comment_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/issues/comments{/number}",
    //"contents_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/contents/{+path}",
    //"compare_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/compare/{base}...{head}",
    //"merges_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/merges",
    //"archive_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/{archive_format}{/ref}",
    //"downloads_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/downloads",
    public string downloads_url;
    //"issues_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/issues{/number}",
    //"pulls_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/pulls{/number}",
    //"milestones_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/milestones{/number}",
    //"notifications_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/notifications{?since,all,participating}",
    //"labels_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/labels{/name}",
    //"releases_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/releases{/id}",
    //"deployments_url": "https://api.github.com/repos/EloiStree/2012_04_29_DesignPatternInUnity/deployments",
    //"created_at": "2020-04-29T18:33:12Z",
    public DateTime GetCreatedDate() { return PublicGitLab.ConvertToDate(created_at); }
    public string created_at;
    //"updated_at": "2020-04-29T18:45:17Z",
    public DateTime GetUpdateDate() { return PublicGitLab.ConvertToDate(updated_at); }
    public string updated_at;
    //"pushed_at": "2020-04-29T18:45:15Z",
    public DateTime GetPushedDate() { return PublicGitLab.ConvertToDate(pushed_at); }
    public string pushed_at;
    //"git_url": "git://github.com/EloiStree/2012_04_29_DesignPatternInUnity.git",
    public string git_url;
    //"ssh_url": "git@github.com:EloiStree/2012_04_29_DesignPatternInUnity.git",
    public string ssh_url;
    //"clone_url": "https://github.com/EloiStree/2012_04_29_DesignPatternInUnity.git",
    public string clone_url;
    //"svn_url": "https://github.com/EloiStree/2012_04_29_DesignPatternInUnity",
    public string svn_url;
    //"homepage": null,
    //"size": 3,
    //"stargazers_count": 0,
    //"watchers_count": 0,
    //"language": null,
    //"has_issues": true,
    public bool has_issues;
    //"has_projects": true,
    public bool has_projects;
    //"has_downloads": true,
    public bool has_downloads;
    //"has_wiki": true,
    public bool has_wiki;

    //"has_pages": false,
    //"forks_count": 0,
    public int forks_count;
    //"mirror_url": null,
    //"archived": false,
    //"disabled": false,
    //"open_issues_count": 1,
    public int open_issues_count;
    //"license": null,
    //"forks": 0,
    //"open_issues": 1,
    public int open_issues;
    //"watchers": 0,
    public int watchers;
    //"default_branch": "master"
    public string default_branch;

}

[System.Serializable]
public class JsonListGitLabProjects {
    public JsonGitLabProject[] values;
}
[System.Serializable]
public class JsonGitLabProject
{
    //"name": "2019_06_03_AdbUtility",
    public string name;
    //"id": 12674541,
    public string id;
    //"description": "",
    public string description;
    //"name_with_namespace": "Stree Eloi / 2019_06_03_AdbUtility",
    public string name_with_namespace;
    //"path": "2019_06_03_adbutility",
    public string path;
    //"path_with_namespace": "eloistree/2019_06_03_adbutility",
    public string path_with_namespace;
    //"created_at": "2019-06-03T15:33:04.311Z",
    public string created_at;
    //"default_branch": "master",
    public string default_branch;
    //"tag_list": [],
    public string [] tag_list;
    //"ssh_url_to_repo": "git@gitlab.com:eloistree/2019_06_03_adbutility.git",
    public string ssh_url_to_repo;
    //"http_url_to_repo": "https://gitlab.com/eloistree/2019_06_03_adbutility.git",
    public string http_url_to_repo;
    //"web_url": "https://gitlab.com/eloistree/2019_06_03_adbutility",
    public string web_url;
    //"readme_url": "https://gitlab.com/eloistree/2019_06_03_adbutility/-/blob/master/README.md",
    public string readme_url;
    //"avatar_url": null,
    public string avatar_url;
    //"star_count": 0,
    public string star_count;
    //"forks_count": 0,
    public string forks_count;
    //"last_activity_at": "2019-09-21T20:43:38.685Z",
    public string last_activity_at;
    //"namespace": {
    //"id": 2671690,
    //"name": "Stree Eloi",
    //"path": "eloistree",
    //"kind": "user",
    //"full_path": "eloistree",
    //"parent_id": null,
    //"avatar_url": "/uploads/-/system/user/avatar/2148054/avatar.png",
    //"web_url": "https://gitlab.com/eloistree"
    //}


}
