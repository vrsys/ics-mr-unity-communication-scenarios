     #if UNITY_EDITOR
     using UnityEngine;
     using UnityEditor;
     #endif
     
     public class StarPrimitive {
         #if UNITY_EDITOR
         private static Mesh CreateMesh() {
         Vector3[] vertices = new Vector3[10];

        float radius = 0.5f;
        float angle = 2 * Mathf.PI / 5f;

        for (int i = 0; i < 5; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), radius * Mathf.Cos(i * angle), 0f);
        }

        radius *= 0.4f; 

        for (int i = 0; i < 5; i++)
        {
            float a = 0.5f + i;
            vertices[5+i] = new Vector3(radius * Mathf.Sin(a * angle), radius * Mathf.Cos(a * angle), 0f);
        }

        Vector2[] uv = new Vector2[10];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

             int[] triangles = { 0,5,9,1,6,5,2,7,6,3,8,7,4,9,8,7,8,9,9,5,6,9,6,7 };
     
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
             var obj = new GameObject("Star");
             var mesh = CreateMesh();
             var filter = obj.AddComponent<MeshFilter>();
             var renderer = obj.AddComponent<MeshRenderer>();
             var collider = obj.AddComponent<MeshCollider>();
     
             filter.sharedMesh = mesh;
             collider.sharedMesh = mesh;
             renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
     
             return obj;
         }
     
         [MenuItem("GameObject/3D Object/Star", false, 0)]
         public static void Create() {
             CreateObject();
         }
         #endif
     }