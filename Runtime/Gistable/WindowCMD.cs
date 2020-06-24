using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public class WindowCMD
{
    public static void RunCommands(string[] cmds, string workingDirectory, bool useDebug = false)
    {
        WindowCMDCallback dontcare;
         RunCommands(cmds, workingDirectory, useDebug, out dontcare);
    }

    public static void RunCommands(string[] cmds, string workingDirectory, bool useDebug , out WindowCMDCallback callback )
    {
        callback = new WindowCMDCallback();
        WindowCMDCallbackRef callbackRef = new WindowCMDCallbackRef(callback);
        callback.SetUsedDirectory(workingDirectory);
        callback.AddCommandsToUsed(cmds);
        callback.StartExecuting();
        try
        {
            if (workingDirectory.Length < 2) return;

            char disk = 'C';
            if (workingDirectory[1] == ':')
                disk = workingDirectory[0];

            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            //psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WorkingDirectory = workingDirectory;
            process.StartInfo = psi;
            process.Start();
            process.OutputDataReceived += (sender, e) =>
            {
                callbackRef.m_callback.AddConsoleCallback(e.Data);
                if (useDebug)
                    UnityEngine.Debug.Log(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                callbackRef.m_callback.AddConsoleError(e.Data);
                if (useDebug)
                    UnityEngine.Debug.Log(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using (StreamWriter sw = process.StandardInput)
            {
                sw.WriteLine(disk + ":");
                sw.WriteLine("cd " + workingDirectory);
                foreach (var cmd in cmds)
                {
                    callbackRef.m_callback.SetAsExecutingCommand(cmd);
                    if (useDebug)
                        UnityEngine.Debug.Log("> " + cmd);
                    sw.WriteLine(cmd);
                    callbackRef.m_callback.AddExecutedCommand(cmd);
                }
            }
            process.WaitForExit();
        }
        catch (Exception e)
        {
            callback.NotifyError(e.StackTrace);
        }
        callback.RemoveMicrosfotReservedLabelOfConsoleCallback();
        callback.RemoveSentInfoOfConsoleCallback();
        callback.SetAsFinished();
    }
}

public class WindowCMDCallbackRef {
    public WindowCMDCallbackRef(WindowCMDCallback callback) {
        m_callback = callback;
    }
    public WindowCMDCallback m_callback;
}
[Serializable]
public class WindowCMDCallback
{
    public List<string> m_cmdToSend = new List<string>();
    public string m_commandInProcess;
    public List<string> m_cmdSent = new List<string>();
    public List<string> m_appendText = new List<string>();
    public List<string> m_errors = new List<string>();
    public bool m_isExecuting = false;
    public bool m_finishedExecuting = false;
    public string m_exceptionErrorMessage = "";
    public string m_useDirectoryForExecuting;
    public void NotifyError(string stackTrace)
    {
        m_exceptionErrorMessage = stackTrace;
    }

    public void AddCommandsToUsed(string[] cmds)
    {
        m_cmdToSend.AddRange(cmds);
    }
   

    public void StartExecuting()
    {
        m_isExecuting = true;
        m_finishedExecuting = false;
    }
    public void SetAsFinished()
    {
        m_isExecuting = false;
        m_finishedExecuting = true;
    }

    public void AddConsoleError(string consoleText)
    {
        if (string.IsNullOrEmpty(consoleText)) return;
        m_errors.Add(consoleText);
    }

    public void AddConsoleCallback(string consoleText)
    {
        if (string.IsNullOrEmpty(consoleText)) return;
        m_appendText.Add(consoleText);
    }

    public void AddExecutedCommand(string cmd)
    {
        m_cmdSent.Add(cmd);
    }

    public void SetAsExecutingCommand(string cmd)
    {
        m_commandInProcess = cmd;
    }
    public void RemoveMicrosfotReservedLabelOfConsoleCallback()
    {
        //Microsoft Windows[Version 10.0.18363.900]
        //(c) 2019 Microsoft Corporation. All rights reserved.
        m_appendText = m_appendText.Where(k =>
        k.ToLower().IndexOf("(c) 2019 Microsoft Corporation. All rights reserved.".ToLower()) < 0).ToList();
        m_appendText = m_appendText.Where(k =>
        !k.ToLower().StartsWith("Microsoft Windows".ToLower()) ).ToList();
    }
    public void RemoveSentInfoOfConsoleCallback()
    {
        //Microsoft Windows[Version 10.0.18363.900]
        //(c) 2019 Microsoft Corporation. All rights reserved.
        m_appendText = m_appendText.Where(k => !k.StartsWith(m_useDirectoryForExecuting)).ToList();
    }

    internal void SetUsedDirectory(string workingDirectory)
    {
        m_useDirectoryForExecuting = workingDirectory;
    }

    public string[] GetReceivedTextAsLines()
    {
        return m_appendText.ToArray();
    }
}