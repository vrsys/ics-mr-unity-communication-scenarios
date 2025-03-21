     #if UNITY_EDITOR
     using UnityEngine;
     using UnityEditor;
     #endif
     
     public class HexagonPrimitive {
         #if UNITY_EDITOR
         private static Mesh CreateMesh() {
         Vector3[] vertices = new Vector3[6];

        float radius = 0.5f;
        float angle = 2 * Mathf.PI / 6f;

        for (int i = 0; i < 6; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), radius * Mathf.Cos(i * angle), 0f);
        }

        Vector2[] uv = {
                 new Vector2(-0.5f, 0f),
                 new Vector2(-0.25f, 0.5f),
                 new Vector2(0.25f, 0.5f),
                 new Vector2(0.5f, 0f),
                 new Vector2(0.25f, -0.5f),
                 new Vector2(-0.25f, -0.5f)
             };
     
             int[] triangles = { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 };
     
             var mesh = new Mesh();
             mesh.vertices = vertices;
             mesh.uv = uv;
             mesh.triangles = triangles;
             mesh.RecalculateBounds();
             mesh.RecalculateNormals();
             mesh.RecalculateTangents();
             return mesh;
         }
     
         private static GameObject CreateObject() {
             var obj = new GameObject("Hexagon");
             var mesh = CreateMesh();
             var filter = obj.AddComponent<MeshFilter>();
             var renderer = obj.AddComponent<MeshRenderer>();
             var collider = obj.AddComponent<MeshCollider>();
     
             filter.sharedMesh = mesh;
             collider.sharedMesh = mesh;
             renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");

            string path = "Assets/GeneratedMeshes/HexagonMesh.asset";
            SaveAsset.SaveMeshAsset(mesh, path);

            return obj;
         }
     
         [MenuItem("GameObject/3D Object/Hexagon", false, 0)]
         public static void Create() {
             CreateObject();
         }
         #endif
     }