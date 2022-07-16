using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Compilation;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Lachee.Tools.Editor
{
    /// <summary>
    /// Processes C# scripts to add namespaces automatically
    /// </summary>
    public class NamespaceProcessor : UnityEditor.AssetModificationProcessor
    {

        private readonly static Regex _namespaceRegex = new Regex(@"namespace\s(\s?[a-zA-Z]+[0-9]*\.?)*", RegexOptions.Compiled);

        /// <summary>
        ///  This gets called for every .meta file created by the Editor.
        /// </summary>
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);
            if (!path.EndsWith(".cs")) return;

            // Get the desired namespace and if we should be editing it
            string desiredNamespace = GetNamespace(path, out var configuration);
            if (configuration == null || !configuration.IsAutomatic) return;

            var systemPath = path.Insert(0, Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")));
            SetNamespace(path, desiredNamespace);

            if (configuration.FormatDocument)
                FormatScripts(path);

            AssetDatabase.Refresh();
        }
        
        /// <summary>Sets / Replaces the namespace of a given file</summary>
        public static void SetNamespace(string asset, string @namespace)
        {
            if (Path.GetExtension(asset) != ".cs")
                throw new System.ArgumentException("Asset must be a cs file", "asset");

            string contents = File.ReadAllText(asset);
            if (contents.Contains("namespace"))
            {
                contents = _namespaceRegex.Replace(contents, "namespace " + @namespace);
                // TODO: Refactor
            }
            else
            {
                int index = FindIndexOfLastImport(contents);
                contents = contents.Insert(index, "\nnamespace " + @namespace + " {");
                contents += "\n}";
            }
            File.WriteAllText(asset, contents);
        }
        private static int FindIndexOfLastImport(string content)
        {
            int index = 0;
            bool inComment = false;
            do
            {
                int nindex = content.IndexOf('\n', index + 1);
                if (nindex <= 0) break;

                // Get the line
                string line = content.Substring(index, nindex - index).Trim();

                if (line.StartsWith("/*"))
                    inComment = true;

                if (line.EndsWith("*/"))
                    inComment = false;

                // Skip comments or empty lines
                if (!inComment && !string.IsNullOrEmpty(line) && !line.StartsWith("//"))
                {
                    // Check if it starts with anything but using (and isnt empty)
                    if (!line.StartsWith("using") && !line.StartsWith("use"))
                        return index;
                }

                //Update the index and move on
                index = nindex;
            } while (index > 0 && index < content.Length);
            return index;
        }

        /// <summary>Maps the path of a namespace with the actual namespace</summary>
        public static IReadOnlyDictionary<string, NamespaceConfiguration> GetRootNamespaceConfigurations()
        {
            // Map all configurations 
            Dictionary<string, NamespaceConfiguration> configurations = new Dictionary<string, NamespaceConfiguration>();
            foreach (var guid in AssetDatabase.FindAssets("t:NamespaceConfiguration"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var configuration = AssetDatabase.LoadAssetAtPath<NamespaceConfiguration>(path);
                configurations[Path.GetDirectoryName(path)] = configuration;
            }

            return configurations;
        }

        /// <summary>Namespace for newly created configurations</summary>
        internal static string GetNamespaceForNewConfiguration(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return GetDefaultNamespace();

            // we ignore the out because we dont care if we find a NamespaceConfiguration or not
            string root = System.IO.Path.GetDirectoryName(path).Split('\\', '/').Last();
            string recommended = NamespaceProcessor.GetNamespace(path, out var _, path);
            return root == "Assets" || recommended.EndsWith(root) ? recommended : $"{recommended}.{root}";
        }

        /// <summary>
        /// Gets the best namespace for the given file.
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        /// <param name="configuration">The configuration that was used to create the namespace. It will be null if it was created using other means.</param>
        /// <returns></returns>
        public static string GetNamespace(string filePath, out NamespaceConfiguration configuration, string excludePath = "")
        {
            configuration = null;

            const int MAX_ITERATION = 2048;
            int iteration = 0;

            // Prepare the path
            string dir = GetProjectDirectoryPath(filePath);
            if (!dir.StartsWith("Assets")) 
                return GetDefaultNamespace();

            // Try to find the best fit configuration
            string current = dir;
            var configurations = GetRootNamespaceConfigurations();
            while (current.StartsWith("Assets") && (iteration++ < MAX_ITERATION))
            {
                if (configurations.TryGetValue(current, out configuration))
                {
                    // Make sure we are not using ourself!
                    string configurationPath = AssetDatabase.GetAssetPath(configuration);
                    if (configurationPath != excludePath)
                    {

                        // Include the sub directories?
                        if (configuration.IncludeSubDirectories)
                            return string.Join(".", configuration.RootNamespace, GetNamespaceFromDirectory(GetRelativeDirectory(current, dir))).Trim('.', ' ');

                        // Return just the namespace as is otherwise.
                        return configuration.RootNamespace.Trim('.', ' ');
                    }
                }

                    current = Path.GetDirectoryName(current);
            }

            // Try to find the assembly definition root
            string assemblyRootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(filePath);
            if (!string.IsNullOrWhiteSpace(assemblyRootNamespace))
            {
                return assemblyRootNamespace;
            }

            // Give up and just use the company name and product name
            return GetDefaultNamespace();
        }

        /// <summary>
        /// Gets the default namespace for the file within the project
        /// </summary>
        /// <param name="projectDirectory">The directory within the project folder</param>
        /// <returns></returns>
        private static string GetDefaultNamespace()
        {
            string productName = PascalCase(Application.companyName) + "." + PascalCase(Application.productName);
            return productName.Trim('.', ' ');
        }

        /// <summary>Converts the directory into a valid namespace</summary>
        private static string GetNamespaceFromDirectory(string directory)
            => string.Join(".", directory.Split('\\', '/').Select(s => PascalCase(s)));

        /// <summary>Gets the directory relative to the project the file is within</summary>
        private static string GetProjectDirectoryPath(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(directoryPath)) return "Assets";
            if (directoryPath.StartsWith("Assets")) return directoryPath;

            return GetRelativeDirectory(Application.dataPath, directoryPath);
        }

        /// <summary>Gets the directory relative</summary>
        private static string GetRelativeDirectory(string from, string too)
        {
            too = too.Replace('\\', '/');
            from = from.Replace('\\', '/');
            return too.Replace(from, "").TrimStart('/');
        }

        private static string PascalCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return string.Join("", str.Split(new char[] { '-', '_', ' ' }).Select(i => char.ToUpper(i[0]) + i.Substring(1)));
        }


        /// <summary>Spawns a Dotnet process to format a list of assets</summary>
        public static void FormatScripts(IEnumerable<string> assets)
        {
            //Run dotnet format on everything
            var args = "format --include " + string.Join(" ", assets.Select(a => '"' + GetRelativeDirectory("Assets/", a) + '"')) + " -f";
            using (var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = args,
                WorkingDirectory = Application.dataPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            }))
            {

                process.EnableRaisingEvents = true;
                process.OutputDataReceived += (sender, e) => { UnityEngine.Debug.Log(e.Data); };
                process.ErrorDataReceived += (sender, e) => { UnityEngine.Debug.LogError(e.Data); };
                process.WaitForExit();
            }
        }
        public static void FormatScripts(params string[] assets)
            => FormatScripts((IEnumerable<string>)assets);
    }
}