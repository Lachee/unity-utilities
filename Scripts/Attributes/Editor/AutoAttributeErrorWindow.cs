#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Lachee.Attributes.Editor
{
    public class AutoAttributeErrorWindow : EditorWindow
    {
        private static AutoAttributeErrorWindow _instance;
        public static AutoAttributeErrorWindow Instance => _instance;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Tools/Auto Errors")]
        public static void ShowWindow()
        {
            if (!_instance)
            {
                _instance = EditorWindow.CreateWindow<AutoAttributeErrorWindow>(new System.Type[] { System.Type.GetType("UnityEditor.ConsoleWindow,UnityEditor.dll") });
                _instance.titleContent = new GUIContent("Auto Errors");
            }
           _instance.Focus();
        }

        private void OnEnable()
        {
            // Update all the attributes, processing their errors again
            AutoAttributeObserver.ProcessAttributes();
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

                var playModeBlockPref = EditorPrefs.GetBool(AutoAttributeObserver.PREF_PREVENT_PLAYMODE, true);
                var playModeBlockPrefEdit = EditorGUILayout.Toggle("Block PlayMode", playModeBlockPref);
                if (playModeBlockPref != playModeBlockPrefEdit)
                    EditorPrefs.SetBool(AutoAttributeObserver.PREF_PREVENT_PLAYMODE, playModeBlockPrefEdit);

                if (GUILayout.Button("Manually Scan Attributes", GUILayout.MaxWidth(200f)))
                    AutoAttributeObserver.ProcessAttributes();

                EditorGUILayout.Space(10f);

                // Draw the errors
                EditorGUILayout.LabelField("Errors", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginVertical();
                foreach (var error in AutoAttributeObserver.errors)
                {
                    DrawErrorLayout(error);
                    EditorGUILayout.Space(10f);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawErrorLayout(AutoAttributeObserver.ObjectError errorObject)
        {
            SerializedObject serializedObject = new SerializedObject(errorObject.component);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("Component", errorObject.component, errorObject.component.GetType(), true);
            if (GUILayout.Button("Reveal in Inspector", GUILayout.Width(175f)))
            {
                Selection.activeObject = errorObject.component;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 200f;
            foreach (var kp in errorObject.errors)
            {
                var path = kp.Key;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(path.propertyPath));
            }
            EditorGUILayout.EndVertical();
        }
    }
}
#endif