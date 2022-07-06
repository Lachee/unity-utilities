using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;

namespace Lachee.Tools.Editor 
{
    /// <summary>
    /// Provides tools to convert line endings
    /// </summary>
    public class EOLConversion
    {
        [MenuItem("Tools/EOL Conversion/Windows")]
        private static void ConvertToWindows()
        {
            Convert("\r\n");
        }

    
        [MenuItem("Tools/EOL Conversion/Unix")]
        private static void ConvertToUnix()
        {
            Convert("\n");
        }

        /// <summary> Converts all the assets and returns a list of files that were modified </summary>
        public static string[] Convert(string lineEnding) {
            List<string> assetsConverted = new List<string>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            int progress = 0;

            foreach(string assetPath in assetPaths)
            {
                EditorUtility.DisplayProgressBar("Converting Line Ending", assetPath, (progress++ / (float)assetPaths.Length));

                if(!assetPath.EndsWith(".cs")) continue;
                if (assetPath.StartsWith("Packages/")) continue;


                string content      = File.ReadAllText(assetPath);
                string contentNew   = Regex.Replace(content, @"\r\n|\n\r|\n|\r", lineEnding);

                if (content != contentNew) {
                    File.WriteAllText(assetPath, contentNew);
                    assetsConverted.Add(assetPath);
                }
            }
        
            EditorUtility.ClearProgressBar();
            return assetsConverted.ToArray();
        }
    }

}