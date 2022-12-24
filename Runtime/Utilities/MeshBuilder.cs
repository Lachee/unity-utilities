using UnityEngine;
using System.Collections.Generic;

namespace Lachee.Utilities
{
    /// <summary>
    /// The MeshBuilder allows for easier manipulation and creation of meshes.
    /// <para>This used to be called the MeshDetails</para>
    /// </summary>
    public class MeshBuilder
    {
        private List<Vector3> verts;
        private List<Vector2> uvs;
        private List<int> tris;
        private List<Color> colors;

        private Dictionary<int, List<int>> submesh;
        public int submeshCount { get { return submesh.Count; } }

        private Vector3 center;
        private bool hasCenter = true;

        #region Constructor
        public MeshBuilder()
        {
            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector2>();
            colors = new List<Color>();
            submesh = new Dictionary<int, List<int>>();

            hasCenter = false;
        }
    
        public MeshBuilder(int vertexCount, int triangleCount)
        {
            verts = new List<Vector3>(vertexCount);
            tris = new List<int>(triangleCount);
            uvs = new List<Vector2>(vertexCount);
            colors = new List<Color>(vertexCount);
            submesh = new Dictionary<int, List<int>>();

            hasCenter = false;
        }

        public MeshBuilder(Mesh m)
        {

            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector2>();
            colors = new List<Color>();

            verts.AddRange(m.vertices);
            uvs.AddRange(m.uv);
            tris.AddRange(m.triangles);
            colors.AddRange(m.colors);

            //Add the submeshes
            for (int i = 0; i < m.subMeshCount; i++)
                submesh.Add(i, new List<int>(m.GetTriangles(i)));


            hasCenter = false;
        }
        public MeshBuilder(MeshBuilder a, MeshBuilder b)
        {
            //Add A's vertex, triangles, uvs, colors and submeshes
            verts = a.verts;
            tris = a.tris;
            uvs = a.uvs;
            colors = a.colors;
            submesh = a.submesh;

            //Capture index of the current vertexs
            int index = verts.Count;

            //Add B's vertex, uvs and colors
            verts.AddRange(b.verts);
            uvs.AddRange(b.uvs);
            colors.AddRange(b.colors);

            //Check if b has more submeshes
            if (b.submeshCount > 1)
                Debug.LogWarning("Submesh addition is not yet supported! Compressing all submeshes as one triangle instead!");

            //Prepare the triangles
            int[] btris = new int[b.tris.Count];
            for (int i = 0; i < b.tris.Count; i++)
            {
                btris[i] = index + b.tris[i];
                tris.Add(btris[i]);
            }

            //Get the current submesh ID
            int s_index = submesh.Count;

            //Add it to the submesh
            submesh.Add(s_index, new List<int>(btris));

            hasCenter = false;
        }
        #endregion

        #region Adders
        public void AddVert(Vector2 v, float z = 0f) { verts.Add(new Vector3(v.x, v.y, z)); hasCenter = false; }
        public void AddVert(Vector3 v) { verts.Add(v); hasCenter = false; }
        public void AddVert(params Vector3[] vectors)
        {
            verts.AddRange(vectors);
            hasCenter = false;
        }

        public void AddTris(int a, int b, int c, int s = 0) { AddTri(a, s); AddTri(b, s); AddTri(c, s); }
        public void AddTri(int a, int s = 0)
        {
            tris.Add(a);

            if (s >= submeshCount) submesh.Add(s, new List<int>());
            submesh[s].Add(a);
        }


        public void AddColor(Color c) { colors.Add(c); }
        public void AddUV(Vector2 uv) { uvs.Add(uv); }
        public void AddUV(params Vector2[] uvs) { this.uvs.AddRange(uvs); }

        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            AddVert(a);
            AddVert(b);
            AddVert(c);
            CreateTriangle();
        }

        /// <summary>
        /// Creates a rectangle from the given points. Automatically uses correct faces.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public void AddRectangle(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            int index = CountVerts();
            AddVert(a);
            AddVert(b);
            AddVert(c);
            AddVert(d);
            AddTris(index + 0, index + 1, index + 2);
            AddTris(index + 3, index + 2, index + 1);
        }

        #endregion

        #region Setters

        public void SetTris(IEnumerable<int> polyTris, int s = 0)
        {
            this.tris.Clear();
            this.tris.AddRange(polyTris);

            if (s >= submeshCount)
                submesh.Add(s, new List<int>());
            else
                submesh[s].Clear();

            submesh[s].AddRange(polyTris);
        }
        public void SetVerts(IEnumerable<Vector3> verts)
        {
            this.verts.Clear();
            this.verts.AddRange(verts);
            this.hasCenter = false;
        }
        public void SetColors(IEnumerable<Color> colors)
        {
            this.colors.Clear();
            this.colors.AddRange(colors);
        }
        public void SetUVs(IEnumerable<Vector2> uvs)
        {
            this.uvs.Clear();
            this.uvs.AddRange(uvs);
        }
        #endregion

        #region Counters
        public int CountVerts() { return verts.Count; }
        public int CountTris() { return tris.Count; }
        public int CountUVs() { return uvs.Count; }
        public int CountColors() { return colors.Count; }
        #endregion

        #region Getters
        public Vector3 GetVert(int i)
        {
            if (i < 0 || i >= verts.Count) return Vector3.zero;
            return verts[i];
        }
        public Vector3[] GetVerts(int[] tris)
        {
            Vector3[] vectors = new Vector3[tris.Length];
            for (int i = 0; i < tris.Length; i++) vectors[i] = GetVert(tris[i]);
            return vectors;
        }
        public int GetTri(int i)
        {
            if (i < 0 || i >= tris.Count) return -1;
            return tris[i];
        }
        public int[] GetTris(int i)
        {
            if (i < 0 || i + 2 >= tris.Count) return null;
            return new int[] { tris[i], tris[i + 1], tris[i + 2] };
        }
        public Vector2 GetUV(int i)
        {
            if (i < 0 || i >= uvs.Count) return Vector2.zero;
            return uvs[i];
        }
        public Color GetColor(int i)
        {
            if (i < 0 || i >= colors.Count) return Color.magenta;
            return colors[i];
        }
        public Vector3 GetCenter()
        {
            return hasCenter ? center : CalculateCenter();
        }
        #endregion

        #region Manipulation

        /// <summary>
        /// Moves the center of the mesh to the position. Moving the verts appropriately
        /// </summary>
        /// <param name="position">The position to center around</param>
        public void CenterMesh(Vector3 position)
        {
            //The current center
            Vector3 center = GetCenter();

            // go through each vert
            for (int i = 0; i < verts.Count; i++)
            {
                //Calcaulte its offset
                Vector3 vert = verts[i];
                Vector3 offset = vert - center;

                //Calculate its new position
                Vector3 nvert = position + offset;

                //Apply it
                verts[i] = nvert;
            }

            //We no longer has a valid center. Could update it, but meh
            hasCenter = false;
        }
        public Vector3 CalculateCenter()
        {
            //Prepare the sums
            float xsum = 0, ysum = 0, zsum = 0;

            //Calcaulte all the sums
            foreach (Vector3 vert in verts)
            {
                xsum += vert.x;
                ysum += vert.y;
                zsum += vert.z;
            }

            //Calcalculate the adverages
            center = new Vector3(xsum, ysum, zsum) / verts.Count;
            hasCenter = true;

            //return the center
            return center;
        }
        
        public void CreateQuad()
        {
            int c = CountVerts();
            if (c < 4) return;

            CreateTriangle(c - 1, c - 2, c - 3);
            CreateTriangle(c - 1, c - 3, c - 4);
        }
        public void CreateTriangle()
        {
            if (CountVerts() < 3)
                return;

            CreateTriangle(CountVerts() - 1, CountVerts() - 2, CountVerts() - 3);
        }
        public void CreateTriangle(int a, int b, int c)
        {
            Vector3 av = verts[a];
            Vector3 bv = verts[b];
            Vector3 cv = verts[c];

            Vector3 side1 = bv - av;
            Vector3 side2 = cv - av;

            Vector3 perp = Vector3.Cross(side1, side2);

            if (perp.z > 0) AddTris(c, b, a); else AddTris(a, b, c);
        }
        public void SetColor(Color c)
        {
            colors.Clear();
            for (int i = 0; i < CountVerts(); i++)
                colors.Add(c);
        }
        public static MeshBuilder operator +(MeshBuilder a, MeshBuilder b)
        {
            if (b == null)
                return a;

            return new MeshBuilder(a, b);
        }


        #endregion

        #region Returns
        public Mesh Create(string name = "Generic Mesh", bool useSubmesh = false, bool isDynamic = false)
        {
            Mesh m = new Mesh();
            m.name = name;
            if (isDynamic) m.MarkDynamic();

            this.Apply(m, useSubmesh);
            return m;
        }
        public void Apply(Mesh mesh, bool useSubmesh = false)
        {
            mesh.triangles = new int[0];

            mesh.vertices = verts.ToArray();
            mesh.colors = colors.ToArray();
            mesh.uv = uvs.ToArray();

            if (!useSubmesh)
            {
                mesh.triangles = tris.ToArray();
            }
            else
            {
                mesh.subMeshCount = submeshCount;
                for (int i = 0; i < submeshCount; i++)
                    mesh.SetTriangles(submesh[i], i);
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Merges 2 MeshDetails together with each one acting as a seperate submesh
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Mesh Merge(MeshBuilder a, MeshBuilder b)
        {
            MeshBuilder q = new MeshBuilder();
            q.verts = a.verts;
            q.uvs = a.uvs;
            q.colors = a.colors;

            int index = q.verts.Count;

            q.verts.AddRange(b.verts);
            q.uvs.AddRange(b.uvs);
            q.colors.AddRange(b.colors);

            int[] btris = new int[b.tris.Count];
            for (int i = 0; i < b.tris.Count; i++)
                btris[i] = b.tris[i] + index;

            Mesh mesh = q.Create();
            mesh.subMeshCount = 2;
            mesh.SetTriangles(a.tris, 0);
            mesh.SetTriangles(btris, 1);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }


        public void Clear()
        {
            verts.Clear();
            tris.Clear();
            colors.Clear();
            uvs.Clear();
            submesh.Clear();
            hasCenter = false;
        }
        #endregion
    }
}