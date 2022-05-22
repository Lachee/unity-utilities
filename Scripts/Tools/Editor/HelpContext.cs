#if UNITY_EDITOR

using System.Diagnostics;
using UnityEditor;

namespace Lachee.Tools.Editor
{
    public class HelpContext : UnityEditor.Editor
    {
        [MenuItem("Help/Scripting Reference Lachee Utils", priority = 100)]
        public static void HelpContextMenu()
        {
            Process.Start("https://lachee.github.io/unity-utilities");
        }        
    }
}
#endif