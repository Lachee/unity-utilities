#if !UNITY_2020_1_OR_NEWER
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ReferenceFilter : EditorWindow
{
    [MenuItem("CONTEXT/Component/Find All References")]
    [MenuItem("GameObject/Component/Find All References")]
    private static void FindReferences(MenuCommand data)
    {
        Object context = data.context;
        if (context)
        {
            var comp = context as Component;
            if (comp)
                FindReferencesTo(comp);
        }
    }

    [MenuItem("Assets/Find All References")]
    private static void FindReferencesToAsset(MenuCommand data)
    {
        var selected = Selection.activeObject;
        if (selected)
            FindReferencesTo(selected);
    }

    private static void FindReferencesTo(Object to)
    {
        var referencedBy = new List<Object>();
        var allObjects = Object.FindObjectsOfType<GameObject>();
        for (int j = 0; j < allObjects.Length; j++)
        {
            var go = allObjects[j];

            if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
            {
                if (PrefabUtility.GetPrefabParent(go) == to)
                {
                    Debug.Log(string.Format("Referenced by prefab ({1}) {0}", go.name, go.GetType()), go);
                    referencedBy.Add(go);
                }
            }

            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var c = components[i];
                if (!c) continue;

                var so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == to)
                        {
                            Debug.Log(string.Format("Referenced by ({1}) {0}: {2}", c.name, c.GetType(), sp.displayName), c);
                            referencedBy.Add(c.gameObject);
                        }
                    }
            }
        }

        if (referencedBy.Any())
            Selection.objects = referencedBy.ToArray();
        else Debug.LogError("No references found in scene");
    }
}
#endif