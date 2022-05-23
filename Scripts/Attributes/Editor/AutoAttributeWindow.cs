#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Lachee.Attributes.Editor
{
    public class AutoAttributeWindow : EditorWindow
    {
        private static AutoAttributeWindow _instance;
        public static AutoAttributeWindow Instance => _instance;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Tools/Auto Errors")]
        public static void ShowWindow()
        {
            if (!_instance)
            {
                _instance = EditorWindow.CreateWindow<AutoAttributeWindow>(new System.Type[] { System.Type.GetType("UnityEditor.ConsoleWindow,UnityEditor.dll") });
                _instance.titleContent = new GUIContent("Auto Errors");
            }
           _instance.Focus();
        }

        private void OnEnable()
        {
            // Update all the attributes, processing their errors again
            AutoAttributeEditor.CheckAttributes();
        }

        private void OnHierarchyChange()
        {
            Repaint();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                // Draw the settings
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {

                    AutoAttributeEditor.PreventPlayMode = EditorGUILayout.Toggle("Block PlayMode", AutoAttributeEditor.PreventPlayMode);
                    AutoAttributeEditor.AutoRefreshInspector = EditorGUILayout.Toggle("Always Refresh Inspected", AutoAttributeEditor.AutoRefreshInspector);
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Manually Scan Attributes", GUILayout.MaxWidth(200f)))
                    AutoAttributeEditor.CheckAttributes();

                EditorGUILayout.Space(10f);

                // Draw the errors
                EditorGUILayout.LabelField("Errors", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginVertical();
                foreach (var error in AutoAttributeEditor.Errors)
                {
                    DrawErrorLayout(error);
                    EditorGUILayout.Space(10f);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawErrorLayout(AutoAttributeEditor.ObjectErrors errors)
        {
            SerializedObject serializedObject = new SerializedObject(errors.Component);
            var gameObject = (errors.Component as Component).gameObject;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(gameObject.name, errors.Component, errors.Component.GetType(), true);
            if (GUILayout.Button("Reveal in Inspector", GUILayout.Width(175f)))
                Selection.activeObject = errors.Component;
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(EditorGUIUtility.labelWidth + 5f);
                EditorGUILayout.BeginVertical();
                {
                    var originalLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 200f;
                    foreach (var kp in errors)
                    {
                        var path = kp.Key;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(path.propertyPath));
                    }
                    EditorGUIUtility.labelWidth = originalLabelWidth;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif