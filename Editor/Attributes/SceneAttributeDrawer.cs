using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lachee.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributeDrawer: PropertyDrawer
    {
        const float HELP_BOX_HEIGHT = 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyIsValid = Validate(property);
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            EditorGUI.BeginChangeCheck();
            Rect sceneBox = new Rect(position.x, position.y, position.width, position.height - (propertyIsValid ? 0 : HELP_BOX_HEIGHT));
            var newScene = EditorGUI.ObjectField(sceneBox, label, oldScene, typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                property.stringValue = newPath;
            }

            if (!propertyIsValid)
            {
                Rect helpBoxRect = new Rect(position.x, position.y + position.height - HELP_BOX_HEIGHT, position.width, HELP_BOX_HEIGHT);

                EditorGUI.HelpBox(helpBoxRect, "The scene is not listed in the Build Settings. Please ensure it has been added.", MessageType.Warning);
                Rect helpButtonRect = new Rect(helpBoxRect.x + EditorGUIUtility.labelWidth, helpBoxRect.position.y, 150, helpBoxRect.height);
                if (GUI.Button(helpButtonRect, "Add to Build Settings")) 
                {
                    var originalScenes = EditorBuildSettings.scenes;
                    var newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
                    System.Array.Copy(originalScenes, newScenes, originalScenes.Length);
                    var sceneToAdd = new EditorBuildSettingsScene(property.stringValue, true);
                    newScenes[newScenes.Length - 1] = sceneToAdd;
                    EditorBuildSettings.scenes = newScenes;
                    EditorApplication.ExecuteMenuItem("File/Build Settings...");
                }
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (!Validate(property)) 
                height += HELP_BOX_HEIGHT;

            return height;
        }

        public bool Validate(SerializedProperty property)
        {
            return EditorBuildSettings.scenes.Any(scene => scene.path == property.stringValue);
        }
    }
}