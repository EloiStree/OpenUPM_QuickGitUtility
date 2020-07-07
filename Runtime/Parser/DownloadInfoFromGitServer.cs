using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public enum GitServer { GitHub, GitLab, Unknow }
public class DownloadInfoFromGitServer
{
  

    public static void LoadNamespaceFromProjectGitLink(string url, out bool found, out string namespaceID, string branchName="master") {
        //https://gitlab.com/eloistree/2020_06_17_VirtualRealityTags.git#9b8e47d0b2a75b8c53c1ed05f7c8048d503055f9
        found = false;
        namespaceID = "";
        string commitName = "";
        int sharpIndex = url.LastIndexOf("#");
        if (sharpIndex >= 0 && sharpIndex + 1 < url.Length)
            commitName = url.Substring(sharpIndex + 1);
        if (commitName.Length > 0)
            branchName = commitName;
        if (sharpIndex >= 0)
            url = url.Substring(0, sharpIndex );

        url = url.ToLower();

        GitServer server = GetServerTypeOfPath(url);

        url = url.Replace(".git", "");
        int startIndex = url.IndexOf(".com/");
        if (startIndex < 0)
            return;
        url = url.Substring(startIndex + 5);

       // Debug.Log("url:" + url);
        string[] tokens = url.Split('/');
        if (tokens.Length <2)
            return;
        string user= tokens[0], project = tokens[1];
       // Debug.Log("keys:" + server + " "+ user + " " + project + " " +branchName);
        LoadNamespaceFromUrl(server, user, project, branchName, out found, out namespaceID);

//        Debug.Log("NP:" + namespaceID);

        //https://gitlab.com/eloistree/2020_05_25_KoFiCount.git
        //https://github.com/EloiStree/2019_07_21_QuickGitUtility
    }

    public static void LoadNamespaceFromUrl(GitServer server,string userName, string projectName, string projectBranch , out bool found, out string namespaceID) {
        string url = "";
        if (server == GitServer. GitLab)
        {
            url = string.Format("https://gitlab.com/{0}/{1}/-/raw/{2}/package.json", userName, projectName, projectBranch);

        }
        else if (server == GitServer.GitHub)
        {
            url = string.Format("https://raw.githubusercontent.com/{0}/{1}/{2}/package.json"
                , userName, projectName, projectBranch);
        }
       // Debug.Log("Url to to search:" + url);
        LoadNamespaceFromUrl(url, out found, out namespaceID);
    } 

    public static void LoadNamespaceFromUrl(string url, out bool found, out string namespaceID) {

        bool succedToLoad;
        string page = DownloadPage2(url, out succedToLoad).ToLower();
        if (succedToLoad)
        { 
            LoadNamespaceFromText(page, out found, out namespaceID);
        }
        else {
            found = false;
            namespaceID = "";
        }
    }

    public static void LoadNamespaceFromText(string text, out bool found, out string namespaceID)
    {
        found = false;
        namespaceID = "";
        string page = text;
        int index = page.IndexOf("\"name\"");
        if (index < 0) return;
        index += 7;

        page = page.Substring(index);
        page = page.Substring(0, page.IndexOf("\n"));
        page = page.Replace(" ", "").Replace(":", "").Replace("\"", "").Replace(",", "");
        page = page.Trim();
        namespaceID = page;
        found = true;
    }

    public static void LoadGitClassicGitLinksInUrl(string givenUrlLink, out List<string> urlsFound)
    {
        //https://raw.githubusercontent.com/EloiStree/UnityToolbox/master/CopyPast/GitLinks/TestGroup.md
        urlsFound = new List<string>();
        string info = DownloadPage(givenUrlLink);
        Debug.Log(info);
        char [] infoAsLow = info.ToLower().ToCharArray();
            bool isIndexGitFormat=false;
            string gitLink=null;
        for (int i = 0; i < infoAsLow.Length; i++)
        {
            List<int> dotGitIndex = new List<int>();
            IsIndexOfGit(ref infoAsLow, i, out isIndexGitFormat);
            if (isIndexGitFormat)
            {
                ExtractLeftToRightPart(ref infoAsLow, i, out gitLink, new char[] { ' ',',','\n' });
               // Debug.Log("True: "+gitLink);
                urlsFound.Add(gitLink);
            }

        }
        /*
         ssh://user@host.xz:port/path/to/repo.git/
* ssh://user@host.xz/path/to/repo.git/
* ssh://host.xz:port/path/to/repo.git/
* ssh://host.xz/path/to/repo.git/
* ssh://user@host.xz/path/to/repo.git/
* ssh://host.xz/path/to/repo.git/
* ssh://user@host.xz/~user/path/to/repo.git/
* ssh://host.xz/~user/path/to/repo.git/
* ssh://user@host.xz/~/path/to/repo.git
* ssh://host.xz/~/path/to/repo.git
* user@host.xz:/path/to/repo.git/
* host.xz:/path/to/repo.git/
* user@host.xz:~user/path/to/repo.git/
* host.xz:~user/path/to/repo.git/
* user@host.xz:path/to/repo.git
* host.xz:path/to/repo.git
* rsync://host.xz/path/to/repo.git/
* git://host.xz/path/to/repo.git/
* git://host.xz/~user/path/to/repo.git/
* http://host.xz/path/to/repo.git/
* https://host.xz/path/to/repo.git/
* /path/to/repo.git/
* path/to/repo.git/
* ~/path/to/repo.git
* file:///path/to/repo.git/
* file://~/path/to/repo.git/

         */
    }

    private static void IsIndexOfGit(ref char[] infoAsLow, int index, out bool isIndexGitFormat)
    {
        //0+3>=lenght3
        if (index + 3 >= infoAsLow.Length)
        {
            isIndexGitFormat = false;
        }
        else {
            isIndexGitFormat = infoAsLow[index] == '.'
            && (infoAsLow[index+1] == 'G' || infoAsLow[index+1] == 'g')
            && (infoAsLow[index+2] == 'I' || infoAsLow[index+2] == 'i')
            && (infoAsLow[index+3] == 'T' || infoAsLow[index+3] == 't');
        }
    }

    private static void ExtractLeftToRightPart(ref char[] infoAsLow, int index, out string gitLink, char[] spliters= null)
    {
        if (spliters == null)
            spliters = new char[] { ' ' };

        int indexLeft, indexRight;
        indexLeft = index;
        indexRight = index;
        bool isCharValide = true;
        while (indexLeft > 0 && isCharValide)
        {
            for (int i = 0; i < spliters.Length; i++)
            {
                if (infoAsLow[indexLeft] == spliters[i])
                {
                    isCharValide = false;
                    break;
                }

            }
            if(isCharValide)
                indexLeft--;
        }
         isCharValide = true;
        while (indexRight < infoAsLow.Length && isCharValide)
        {
            for (int i = 0; i < spliters.Length; i++)
            {
                if (infoAsLow[indexRight] == spliters[i])
                {
                    isCharValide = false;
                    break;
                }

            }
            if(isCharValide)    
                indexRight++;
         }
        StringBuilder sb = new StringBuilder();
        for (int i = indexLeft; i < indexRight ; i++)
        {
            sb.Append(infoAsLow[i]);
        }

        gitLink = sb.ToString();
    }

    public static string DownloadPage(string url)
    {
        try
        {
            WebClient client = new WebClient();
            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            return s;
        }
        catch (Exception) {
            return "";
        }
    }

    public static string DownloadPage2(string url, out bool succedToLoad)
    {
        string result = "";
        succedToLoad = false;
        HttpWebRequest request=null;
        HttpWebResponse response=null;
        Stream receiveStream = null; 
        StreamReader readStream = null;
        try
        {
            string data = "";
            request = (HttpWebRequest)WebRequest.Create(url);

            response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                 receiveStream = response.GetResponseStream();
                 readStream = null;

                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                data = readStream.ReadToEnd();
                succedToLoad = true;
            }
            result= data;
        }
        catch (Exception e)
        {
            succedToLoad = false;
        }
        finally {
            if(response!=null)
                response.Close();
            if (readStream != null)
                readStream.Close();
        }
        return result;
    }

    public  static GitServer GetServerTypeOfPath(string url)
    {
        GitServer server = GitServer.Unknow;
        if (url.IndexOf("gitlab.com") > -1) 
            server = GitServer.GitLab;
        if (url.IndexOf("github.com") > -1) 
            server = GitServer.GitHub;
        return server;

    }

    //GIT LAB
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount.git
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/blob/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/raw/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/blame/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/commits/master/package.json


    //GIT HUB
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility
    //Info
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/blob/master/package.json
    //Blame
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/blame/master/package.json
    //Raw data
    //https://raw.githubusercontent.com/EloiStree/2019_07_21_QuickGitUtility/master/package.json
    //History
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/commits/master/package.json



}
