#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Lachee.Utilities.Editor
{
    /// <summary>
    /// Previews bones on a skinned mesh. 
    /// <para>Originally designed for VRChat Avatars, it will automatically select the first SkinnedMesh to preview.</para>
    /// </summary>
    public class BoneViewer : UnityEditor.EditorWindow
    {
        public Transform root;

        public int maxDepth = 25;
        public bool showLabels = false;
        public bool showAnchors = false;
        public bool showOnlySelectedChild = false;
        public float maxSelectionDistance = 10f;

        public BoneStyle boneStyle = BoneStyle.Line;
        private SceneView sceneView;

        public enum BoneStyle
        {
            Line,
            Cone
        }


        public static Transform AvailableRoot
            => FindObjectsOfType<SkinnedMeshRenderer>()
                .Select(renderer => renderer.rootBone)
                .FirstOrDefault(root => root != null);

        [MenuItem("Window/Animation/Bone Viewer")]
        [MenuItem("Tools/Hierarchy Viewer")]
        public static void OpenWindow()
        {
            BoneViewer.GetWindow<BoneViewer>("Hierarchy Viewer");
        }

        // Window has been selected
        void OnEnable()
        {
            if (root == null)
                root = AvailableRoot;

            // Add (or re-add) the delegate.
            SceneView.duringSceneGui -= this.OnSceneGUI;
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        void OnDisable()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        private void OnGUI()
        {
            GUIContent content;

            EditorGUILayout.HelpBox("View bones and how they are connected. This tool will traverse through the hierarchy, drawing lines between each bones.", MessageType.Info, true);

            // Root Selector
            EditorGUILayout.LabelField("Root Bone", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                content = new GUIContent("Root", "The object to currently render the root of.");
                root = EditorGUILayout.ObjectField("Root Object", root, typeof(Transform), true) as Transform;
                content = EditorGUIUtility.IconContent("d_Refresh");
                if (GUILayout.Button(content, GUILayout.MaxWidth(30)))
                    root = AvailableRoot;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            // Options
            EditorGUILayout.LabelField("Configurations", EditorStyles.boldLabel);
            content = new GUIContent("Bone Style", "How to visualise the bones.");
            boneStyle = (BoneStyle) EditorGUILayout.EnumPopup(content, boneStyle);

            content = new GUIContent("Max Search Depth", "Maximum depth to search the bone tree.");
            maxDepth = Mathf.Max(0, EditorGUILayout.IntSlider(content, maxDepth, 0, 25));

            content = new GUIContent("Show Labels", "Show the labels of each bone");
            showLabels = EditorGUILayout.Toggle(content, showLabels);

            content = new GUIContent("Show Anchors", "Shows a ball where each bone starts");
            showAnchors = EditorGUILayout.Toggle(content, showAnchors);

            content = new GUIContent("Show Only Selected Child", "Shows only the children bones of the selected child. Useful when in Cone mode.");
            showOnlySelectedChild = EditorGUILayout.Toggle(content, showOnlySelectedChild);

            content = new GUIContent("Max Selection Distance", "Maximum distance to be select bones with");
            maxSelectionDistance = EditorGUILayout.Slider(content, maxSelectionDistance, 0f, 150f);
        }

        private DistanceResult _previousDistanceResult;

        void OnSceneGUI(SceneView sceneView)
        {            
            if (!root || !root.gameObject.scene.IsValid())
                return;

            this.sceneView = sceneView;

            // Do your drawing here using Handles.
            if (showOnlySelectedChild && Selection.activeTransform)
            {
                DrawBone(Selection.activeTransform);
                return;
            }

            _previousDistanceResult = DrawBone(root);


            Handles.BeginGUI();
            if (_previousDistanceResult.child && _previousDistanceResult.parent && _previousDistanceResult.distance < maxSelectionDistance)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Handles.color = Color.yellow;
                Handles.Label(_previousDistanceResult.parent.position, "A");
                Handles.DrawLine(_previousDistanceResult.parent.position, _previousDistanceResult.child.position);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Selection.activeTransform =_previousDistanceResult.parent;
                }
            }
            Handles.EndGUI();
        }

        private DistanceResult DrawBone(Transform root)
            => InternalDrawBonesWithDistance(root, maxDepth);

        struct DistanceResult
        {
            public Transform parent;
            public Transform child;
            public float distance;
        }

        private DistanceResult InternalDrawBonesWithDistance(Transform bone, int maxDepth)
        {
            if (showLabels)
                Handles.Label(bone.position, bone.name);

            DistanceResult distanceResult = new DistanceResult()
            {
                parent = bone,
                child = null,
                distance = Mathf.Infinity
            };

            if (maxDepth <= 0)
                return distanceResult;

            if (showAnchors)
            {
                Handles.SphereHandleCap(0, bone.position, Quaternion.identity, 0.01f, EventType.Repaint);
            }

            foreach (Transform child in bone)
            {
                switch(boneStyle)
                {
                    default:
                    case BoneStyle.Line:
                        DrawBoneLine(bone, child);
                        break;
                    case BoneStyle.Cone:
                        DrawBoneCone(bone, child);
                        break;
                }

                // Dertermine if the child is close
                var distance = HandleUtility.DistanceToLine(bone.position, child.position);
                if (distance < distanceResult.distance)
                {
                    distanceResult.child = child;
                    distanceResult.distance = distance;
                }
                    
                // Determine if anyone is closer from the child
                var childDistanceResult = InternalDrawBonesWithDistance(child, maxDepth - 1);
                if (childDistanceResult.distance < distanceResult.distance)
                {
                    distanceResult = childDistanceResult;
                }
            }

            return distanceResult;
        }

        private void DrawBoneLine(Transform bone, Transform child)
        {
            Handles.DrawLine(bone.position, child.position);
        }


        private void DrawBoneCone(Transform bone, Transform child)
        {
            var diff = child.position - bone.position;
            var look = Quaternion.LookRotation(diff.normalized, bone.up);

            Handles.ConeHandleCap(0, bone.position, look, diff.magnitude, EventType.Repaint);

        }

        private Vector3 InseractionPointB(Vector3 circleCenter, Vector3 circleNormal, Vector3 point, Vector3 direction)
        {
            float a = Vector3.SignedAngle(circleCenter - point, direction, circleNormal);
            float w = 0;
            if (a >= 0) w = 180 - 2 * a; // because w + a + a = 180;
            else w = -(180 + 2 * a);
            Vector3 BO = Quaternion.AngleAxis(w, -circleNormal) * (point - circleCenter);
            return circleCenter + BO;
        }

        private Vector3[] IntersectionPointA(Vector3 p1, Vector3 p2, Vector3 center, float radius)
        {
            Vector3 dp = new Vector3();
            Vector3[] sect;
            float a, b, c;
            float bb4ac;
            float mu1;
            float mu2;

            //  get the distance between X and Z on the segment
            dp.x = p2.x - p1.x;
            dp.z = p2.z - p1.z;
            //   I don't get the math here
            a = dp.x * dp.x + dp.z * dp.z;
            b = 2 * (dp.x * (p1.x - center.x) + dp.z * (p1.z - center.z));
            c = center.x * center.x + center.z * center.z;
            c += p1.x * p1.x + p1.z * p1.z;
            c -= 2 * (center.x * p1.x + center.z * p1.z);
            c -= radius * radius;
            bb4ac = b * b - 4 * a * c;
            if (Mathf.Abs(a) < float.Epsilon || bb4ac < 0)
            {
                //  line does not intersect
                return new Vector3[] { Vector3.zero, Vector3.zero };
            }
            mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);
            mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2 * a);
            sect = new Vector3[2];
            sect[0] = new Vector3(p1.x + mu1 * (p2.x - p1.x), 0, p1.z + mu1 * (p2.z - p1.z));
            sect[1] = new Vector3(p1.x + mu2 * (p2.x - p1.x), 0, p1.z + mu2 * (p2.z - p1.z));

            return sect;
        }
    }
}
#endif