using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;

namespace Lachee.Tools.Editor
{
    /// <summary>
    /// Provides tools to convert line endings
    /// </summary>
    public class EOLConversion : UnityEditor.AssetModificationProcessor
    {
        public const string PREFS_PREFERED = "prefered_eol";
        public const string PREFS_PROCESS = "process_eol";

        [MenuItem("Tools/EOL Conversion/Windows")]
        private static void ConvertToWindows()
        {
            EditorPrefs.SetString(PREFS_PREFERED, "\r\n");
            Convert("\r\n");
        }


        [MenuItem("Tools/EOL Conversion/Unix")]
        private static void ConvertToUnix()
        {
            EditorPrefs.SetString(PREFS_PREFERED, "\n");
            Convert("\n");
        }

        [MenuItem("Tools/EOL Conversion/Automaticly Process")]
        private static void ToggleProcessing()
        {
            bool auto = EditorPrefs.GetBool(PREFS_PROCESS, true);
            EditorPrefs.SetBool(PREFS_PROCESS, !auto);
        }

        [MenuItem("Tools/EOL Conversion/Automaticly Process", true)]
        private static bool ToogleProcessingValidation()
        {
            bool auto = EditorPrefs.GetBool(PREFS_PROCESS, true);
            Menu.SetChecked("Tools/EOL Conversion/Automaticly Process", auto);
            return true;
        }

        /// <summary>
        ///  This gets called for every .meta file created by the Editor.
        /// </summary>
        public static void OnWillCreateAsset(string path)
        {
            bool auto = EditorPrefs.GetBool(PREFS_PROCESS, true);
            if (!auto) return;

            path = path.Replace(".meta", string.Empty);
            if (!path.EndsWith(".cs"))
                return;

            if (!EditorPrefs.HasKey(PREFS_PREFERED))
                return;

            string lineEnding = EditorPrefs.GetString(PREFS_PREFERED);
            if (ConvertFile(path, lineEnding))
                AssetDatabase.Refresh();
        }

        /// <summary> Converts all the assets and returns a list of files that were modified </summary>
        public static string[] Convert(string lineEnding)
        {
            List<string> assetsConverted = new List<string>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            int progress = 0;

            foreach (string assetPath in assetPaths)
            {
                EditorUtility.DisplayProgressBar("Converting Line Ending", assetPath, (progress++ / (float)assetPaths.Length));
                if (ConvertFile(assetPath, lineEnding))
                    assetsConverted.Add(assetPath);
            }

            EditorUtility.ClearProgressBar();
            return assetsConverted.ToArray();
        }

        /// <summary>Converts a single file's line ending</summary>
        public static bool ConvertFile(string path)
            => ConvertFile(path, EditorPrefs.GetString(PREFS_PREFERED, "\r\n"));

        /// <summary>Converts a single file's line ending</summary>
        public static bool ConvertFile(string path, string lineEnding)
        {
            if (!path.EndsWith(".cs") || path.StartsWith("Packages/"))
                return false;

            string content = File.ReadAllText(path);
            string contentNew = Regex.Replace(content, @"\r\n|\n\r|\n|\r", lineEnding);

            if (content != contentNew)
            {
                File.WriteAllText(path, contentNew);
                return true;
            }

            return false;
        }
    }
}