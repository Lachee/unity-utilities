using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.Utilities
{
    /// <summary>
    /// Collection of tools to draw GL lines directly in the scene view
    /// </summary>
    public static class GLUtils
    {
        private static Material _lineMaterial;
        private static Material GetLineMaterial()
        {
            if (_lineMaterial != null)
                return _lineMaterial;

            // From: https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/Helpers/DrawGLLine.cs

            // Unity has a built-in shader that is useful for drawing simple colored things
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _lineMaterial.SetInt("_ZWrite", 0);
            return _lineMaterial;
        }

        public static Transform transform;

        public static void DrawLines(Color color, params Vector3[] points)
            => DrawLines(GetLineMaterial(), color, points);
        public static void DrawLines(Material material, Color color, params Vector3[] points)
        {
            material.SetPass(0);
            GL.PushMatrix();
            {
                if (transform)
                    GL.MultMatrix(transform.localToWorldMatrix);

                GL.Begin(GL.LINE_STRIP);
                {
                    GL.Color(color);
                    for (int i = 0; i < points.Length; i++)
                        GL.Vertex(points[i]);
                }
                GL.End();
            }
            GL.PopMatrix();
        }

        /// <summary>Draws a box on the XY axis</summary>
        public static void DrawBox(Color color, Vector3 center, Vector2 size)
            => DrawBox(GetLineMaterial(), color, center, size);
        /// <summary>Draws a box on the XY axis</summary>
        public static void DrawBox(Material material, Color color, Vector3 center, Vector2 size)
        {
            var hs = size / 2f;
            var tl = new Vector3(center.x - hs.x, center.y + hs.y);
            var tr = new Vector3(center.x + hs.x, center.y + hs.y);
            var bl = new Vector3(center.x - hs.x, center.y - hs.y);
            var br = new Vector3(center.x + hs.x, center.y - hs.y);

            // We are doing this, but we unfirl the loop for speed
            //DrawLines(material, color, tl, tr, br, bl, tl);
            material.SetPass(0);
            GL.PushMatrix();
            {
                if (transform)
                    GL.MultMatrix(transform.localToWorldMatrix);

                GL.Begin(GL.LINE_STRIP);
                {
                    GL.Color(color);
                    GL.Vertex(tl);
                    GL.Vertex(tr);
                    GL.Vertex(br);
                    GL.Vertex(bl);
                    GL.Vertex(tl);
                }
                GL.End();
            }
            GL.PopMatrix();
        }


        /// <summary>Draws a circle on the XY axis</summary>
        public static void DrawCircle(Color color, Vector3 center, float radius, int points = 16)
            => DrawCircle(GetLineMaterial(), color, center, radius, points);

        /// <summary>Draws a circle on the XY Axis</summary>
        public static void DrawCircle(Material material, Color color, Vector3 center, float radius, int points) {

            material.SetPass(0);
            GL.PushMatrix();
            {
                if (transform)
                    GL.MultMatrix(transform.localToWorldMatrix);

                GL.Begin(GL.LINE_STRIP);
                {
                    GL.Color(color);
                    for(int i = 0; i < points + 1; i++)
                    {
                        float progression = i / (float)points;
                        float radians = progression * 2 * Mathf.PI;
                        float x = center.x + radius * Mathf.Cos(radians);
                        float y = center.y + radius * Mathf.Sin(radians);
                        GL.Vertex(new Vector3(x, y, center.z));
                    }
                }
                GL.End();
            }
            GL.PopMatrix();
        }

        public static void DrawTest(Transform root, Color color)
            => DrawTest(root, GetLineMaterial(), color);
        public static void DrawTest(Transform root, Material material, Color color)
        {
            material.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(root.localToWorldMatrix);

            GL.Begin(GL.LINES);
            GL.Color(color);

            // start line from transform position
            GL.Vertex(root.position);
            // end line 100 units forward from transform position
            GL.Vertex(root.position + root.forward * 100);

            GL.End();
            GL.PopMatrix();
        }
    }
}