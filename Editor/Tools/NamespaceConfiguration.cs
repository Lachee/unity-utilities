 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Lachee.Tools.Editor
{
    /// <summary>
    /// Automatic namespace configuration
    /// </summary>
    public class NamespaceConfiguration : ScriptableObject
    {
        /// <summary>
        /// The namespace to apply to all files under this configuration
        /// </summary>
        [field: SerializeField]
        public string RootNamespace { get; set; }
        /// <summary>
        /// Indicates if files should automatically be namespaced
        /// </summary>
        [field: SerializeField]
        public bool IsAutomatic { get; set; }
        /// <summary>
        /// Include subdirectories as sub-namespaces
        /// </summary>
        [field: SerializeField]
        public bool IncludeSubDirectories { get; set; }
        /// <summary>
        /// Format the documents with dotnet format after namespacing
        /// </summary>
        [field: SerializeField]
        public bool FormatDocument { get; set; }

        public void Reset()
        {
            RootNamespace = NamespaceProcessor.GetNamespaceForNewConfiguration(AssetDatabase.GetAssetPath(this));
            IsAutomatic = true;
            IncludeSubDirectories = true;
            FormatDocument = true;
        }

        private void OnValidate()
        {
            RootNamespace = RootNamespace.Trim('.', ' ');
        }
    }
}