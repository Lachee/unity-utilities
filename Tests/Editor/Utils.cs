
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

internal static class Utils
{
    private readonly static System.Type inspectorWindowType;
    private readonly static MethodInfo inspectorLockMethod;

    static Utils()
    {
        inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        inspectorLockMethod = inspectorWindowType.GetMethod("SetObjectsLocked", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static EditorWindow CreateInspector()
    {
        var method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.CreateWindow), new[] { typeof(System.Type[]) });
        var generic = method.MakeGenericMethod(inspectorWindowType);
        return generic.Invoke(null, new object[] { new System.Type[] { } }) as EditorWindow;
    }

    public static void LockInspector(EditorWindow window, List<UnityEngine.Object> objects)
    {
        inspectorLockMethod.Invoke(window, new object[] { objects });
    }
}