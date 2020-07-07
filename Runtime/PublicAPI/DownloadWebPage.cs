using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class DownloadWebPage 
{


    public static string DownloadPageWithWebClient(string url)
    {
        try
        {
            WebClient client = new WebClient(); 
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            return s;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static string DownloadPageWithSuccess(string url, out bool succedToLoad, out string errorMessage)
    {
        errorMessage = "";
        string result = "";
        succedToLoad = false;
        HttpWebRequest request = null;
        HttpWebResponse response = null;
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
            else {

                errorMessage = "Status:" + response.StatusDescription;
            }
            result = data;
        }
        catch (Exception e)
        {
            errorMessage = "Exception:" + e.StackTrace;
            succedToLoad = false;
        }
        finally
        {
            if (response != null)
                response.Close();
            if (readStream != null)
                readStream.Close();
        }
        return result;
    }
}
