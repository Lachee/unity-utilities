using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>Lachee's Gizmos</summary>
    public static class Gizmol {

        public static void DrawExclamation(Vector3 position, float scale = 1f)
        {
            Gizmos.DrawCube(position, new Vector3(0.25f, 0.25f, 0.25f) * scale);
            Gizmos.DrawCube(position + Vector3.up * 0.75f * scale, new Vector3(0.25f, 1f, 0.25f) * scale);
        }

        /// <summary> Draws a Gizmos text at the given position </summary>
        public static void Label(Vector3 position, string text) {
#if UNITY_EDITOR
            //Draw the node as a string
            UnityEditor.Handles.Label(position, text);
#endif
        }

        public static void Label(Vector3 position, string format, params object[] objects) {
            Label(position, string.Format(format, objects));
        }
    }

#pragma warning disable 0649, CS0649
    public static class SortedGizmos
    {
#if UNITY_EDITOR
        static List<ICommand> commands = new List<ICommand>(1000);
#endif

        public static Color color { 
            get
            {
                if (correctColor)
                {
                    const float correction = 1.5f;
                    return new Color(_color.r * correction, _color.g * correction, _color.b * correction, _color.a);
                }
                return _color;
            }
            set { _color = value;  }
        }
        private static Color _color;
        public static bool correctColor = false;

        public static void BatchCommit()
        {
#if UNITY_EDITOR
            Camera cam = null;
            var sv = UnityEditor.SceneView.currentDrawingSceneView;
            if (sv != null && sv.camera != null)
            {
                cam = sv.camera;
            }
            else
            {
                cam = Camera.main;
            }
            if (cam != null)
            {
                var mat = cam.worldToCameraMatrix;
                for (int i = 0; i < commands.Count; ++i)
                {
                    commands[i].Transform(mat);
                }
                // sort by z
                var a = commands.ToArray();
                Array.Sort<ICommand>(a, compareCommands);
                // draw
                for (int i = 0; i < a.Length; ++i)
                {
                    a[i].Draw();
                }
            }
            commands.Clear();
#endif
        }

        public static void DrawSphere(Vector3 center, float radius)
        {
#if UNITY_EDITOR
            commands.Add(new DrawSolidSphereCommand
            {
                color = color,
                position = center,
                radius = radius
            });
#endif
        }

        public static void DrawWireSphere(Vector3 center, float radius)
        {
#if UNITY_EDITOR
            commands.Add(new DrawWireSphereCommand
            {
                color = color,
                position = center,
                radius = radius
            });
#endif
        }

        public static void DrawCube(Vector3 center, Vector3 size)
        {
#if UNITY_EDITOR
            commands.Add(new DrawSolidCubeCommand
            {
                color = color,
                position = center,
                size = size
            });
#endif
        }

        public static void DrawWireCube(Vector3 center, Vector3 size)
        {
#if UNITY_EDITOR
            commands.Add(new DrawWireCubeCommand
            {
                color = color,
                position = center,
                size = size
            });
#endif
        }

#if UNITY_EDITOR
        static int compareCommands(ICommand a, ICommand b)
        {
            float diff = a.SortValue - b.SortValue;
            if (diff < 0f) return -1;
            else if (diff > 0f) return 1;
            else return 0;
        }

        interface ICommand
        {
            void Transform(Matrix4x4 worldToCamera);
            void Draw();
            float SortValue { get; }
        }

        struct DrawSolidSphereCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public float radius;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(position, radius);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }

        struct DrawWireSphereCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public float radius;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(position, radius);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }

        struct DrawSolidCubeCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public Vector3 size;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawCube(position, size);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }

        struct DrawWireCubeCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public Vector3 size;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(position, size);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }

        struct DrawSolidMeshCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public Mesh mesh;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawMesh(mesh, position, rotation, scale);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }

        struct DrawWireMeshCommand : ICommand
        {
            public Color color;
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public Mesh mesh;

            private Vector3 transformedPosition;

            public void Transform(Matrix4x4 mat)
            {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawWireMesh(mesh, position, rotation, scale);
            }

            public float SortValue { get { return transformedPosition.z; } }
        }
#endif
    }
#pragma warning restore 0649, CS0649
}