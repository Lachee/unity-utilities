#if UNITY_EDITOR
using UnityEditor;

namespace Lachee.Utilities.Editor
{
    /// <summary>
    /// Fixes Unity generating incorrect solutions for Assembly Definitions.
    /// <para>
    /// Solution provided by <see href="https://forum.unity.com/threads/2019-3-12f1-build-errors.880312/#post-5789368">Unity Forums</see>.
    /// </para>
    /// </summary>
    public class AssemblyDefinitionSolutionProcessor : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            return content.Replace("<ReferenceOutputAssembly>false</ReferenceOutputAssembly>", "<ReferenceOutputAssembly>true</ReferenceOutputAssembly>");
        }
    }
}
#endif