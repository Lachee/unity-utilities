using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Lachee.Tools.Editor
    {
    public class NamespaceEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/C# Script Namespace", false, 100)]
        public static void CreateNamespaceConfiguration()
        {
            string filePath         = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
            string rootNamespace    = NamespaceProcessor.GetNamespaceForNewConfiguration(filePath);
            filePath                += rootNamespace + ".asset";

            NamespaceConfiguration configuration = ScriptableObject.CreateInstance<NamespaceConfiguration>();
            AssetDatabase.CreateAsset(configuration, filePath);
            AssetDatabase.SaveAssets();

            // Update the asset
            configuration.Reset();
            configuration.RootNamespace = rootNamespace;
            AssetDatabase.SaveAssetIfDirty(configuration);
            AssetDatabase.Refresh();

            Selection.activeObject = configuration;
        }

        [MenuItem("Assets/Update Namespace", true)]
        private static bool ContextMenuVerifyMonoScriptValidation() => !Selection.objects.Any(o => !(o is MonoScript));
        [MenuItem("Assets/Update Namespace", priority = 1000)]
        private static void ContextMenuVerifyMonoScript()
        {
            List<string> formatQueue = new List<string>();
            foreach (var obj in Selection.objects)
            {
                string asset = AssetDatabase.GetAssetPath(obj);

                // Get the desired namespace and if we should be editing it
                string desiredNamespace = NamespaceProcessor.GetNamespace(asset, out var configuration);
                if (configuration == null) return;

                var systemPath = asset.Insert(0, Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")));
                NamespaceProcessor.SetNamespace(systemPath, desiredNamespace);

                if (configuration.FormatDocument)
                    formatQueue.Add(systemPath);
            }

            NamespaceProcessor.FormatScripts(formatQueue);
            AssetDatabase.Refresh();
        }
    }
}