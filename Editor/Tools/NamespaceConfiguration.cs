 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Lachee.Tools.Editor
{
    public class NamespaceConfiguration : ScriptableObject
    {
        [field: SerializeField]
        public string RootNamespace { get; set; }
        [field: SerializeField]
        public bool IsAutomatic { get; set; }
        [field: SerializeField]
        public bool IncludeSubDirectories { get; set; }
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