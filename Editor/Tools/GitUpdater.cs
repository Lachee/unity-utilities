using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Packages.com.lachee.utilties.Editor.Tools
{
    class GitUpdater
    {
        internal static string GitPath { get; private set; }
        internal static string PackagePath => Path.GetFullPath("Packages/com.lachee.utilities/");
        static GitUpdater()
        {
#if UNITY_EDITOR_WIN
            GitPath = ExpandPath("git.exe");
#else
            GitPath = ExpandPath("git");
#endif
        }



        [MenuItem("Tools/Lachee/Update via Git", priority = 10000)]
        public static void HelpContextMenu()
        {
            UnityEngine.Debug.Log($"== GIT UPDATE BEGIN == \nGit Path:\t{GitPath}\nProject Path:\t{PackagePath}");
            var proc = Process.Start(new ProcessStartInfo()
            {
                FileName = GitPath,
                Arguments = "pull",
                WorkingDirectory = PackagePath,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            });
            Debug.Log("== GIT UPDATE RESULTS ==\n\n" + proc.StandardOutput.ReadToEnd());
        }

        [MenuItem("Tools/Lachee/Update via Git", true, priority = 10000)]
        public static bool ValidateGitUpdate()
        {
            if (GitPath == null)
                return false;
            if (!PackagePath.Replace('\\', '/').Contains("Packages/com.lachee.utilities/"))
                return false;
            if (!Directory.Exists(PackagePath + ".git"))
                return false;
            return true;
        }

        static string ExpandPath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                return null;
            }
            return Path.GetFullPath(exe);
        }
    }
}
