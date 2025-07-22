namespace Eloi.Git
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class GitMono_CorrectGitConflict : MonoBehaviour
    {
        public string m_filePath;
        public string m_filePathSave;
        public FileAsGitTextChunk m_lastScan;


        [Serializable]
        public class FileAsGitTextChunk
        {
            public List<GitTextChunk> m_textChunks = new();

            public string GetCurrent()
            {
                return GetTextByTypes(GitTextZoneType.Text, GitTextZoneType.Head);
            }

            public string GetIncoming()
            {
                return GetTextByTypes(GitTextZoneType.Text, GitTextZoneType.Incoming);
            }

            public string GetBoth()
            {
                return GetTextByTypes(GitTextZoneType.Text, GitTextZoneType.Head, GitTextZoneType.Incoming);
            }

            private string GetTextByTypes(params GitTextZoneType[] includedTypes)
            {
                HashSet<GitTextZoneType> types = new(includedTypes);
                StringBuilder sb = new();

                foreach (var chunk in m_textChunks)
                {
                    if (types.Contains(chunk.m_gitTextZoneType))
                    {
                        foreach (string line in chunk.m_lines)
                        {
                            sb.AppendLine(line);
                        }
                    }
                }

                return sb.ToString();
            }
        }

        [Serializable]
        public class GitTextChunk
        {
            public GitTextZoneType m_gitTextZoneType;
            public List<string> m_lines = new();

            public GitTextChunk(GitTextZoneType type)
            {
                m_gitTextZoneType = type;
            }

            public void AddLine(string line)
            {
                m_lines.Add(line);
            }
        }

        public enum GitTextZoneType { Text, Head, Incoming }

        [ContextMenu("Correct with Incoming Change")]
        public void CorrectUsingIncomingChange()
        {
            ScanFile(m_filePath);
            SaveFile(m_filePathSave, m_lastScan.GetIncoming());
        }

        [ContextMenu("Correct with Current Change")]
        public void CorrectUsingCurrentChange()
        {
            ScanFile(m_filePath);
            SaveFile(m_filePathSave, m_lastScan.GetCurrent());
        }

        [ContextMenu("Correct with Both")]
        public void CorrectUsingBoth()
        {
            ScanFile(m_filePath);
            SaveFile(m_filePathSave, m_lastScan.GetBoth());
        }

        [ContextMenu("Scan")]
        public void ScanFileInInspector()
        {
            ScanFile(m_filePath);

        }

        private void ScanFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found at path: {filePath}");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            var chunks = new List<GitTextChunk>();
            var currentChunk = new GitTextChunk(GitTextZoneType.Text);
            chunks.Add(currentChunk);

            bool isInHead = false;
            bool isInIncoming = false;

            foreach (string line in lines)
            {
                bool isConflictMarker = false;

                if (line.StartsWith("<<<<<<< "))
                {
                    isInHead = true;
                    isInIncoming = false;
                    isConflictMarker = true;
                }
                else if (line.StartsWith("======="))
                {
                    isInHead = false;
                    isInIncoming = true;
                    isConflictMarker = true;
                }
                else if (line.StartsWith(">>>>>>> "))
                {
                    isInHead = false;
                    isInIncoming = false;
                    isConflictMarker = true;
                }

                GitTextZoneType newType = isInHead ? GitTextZoneType.Head :
                                          isInIncoming ? GitTextZoneType.Incoming :
                                          GitTextZoneType.Text;

                if (currentChunk.m_gitTextZoneType != newType)
                {
                    currentChunk = new GitTextChunk(newType);
                    chunks.Add(currentChunk);
                }

                if (!isConflictMarker)
                {
                    currentChunk.AddLine(line);
                }
            }

            m_lastScan = new FileAsGitTextChunk { m_textChunks = chunks };
        }

        public void SaveFile(string path, string content)
        {
            string folder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(folder);
            File.WriteAllText(path, content);
        }
    }

}