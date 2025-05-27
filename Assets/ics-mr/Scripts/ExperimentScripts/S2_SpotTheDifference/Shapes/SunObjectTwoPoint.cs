     #if UNITY_EDITOR
     using UnityEngine;
     using UnityEditor;
     #endif
     
     public class TwoPointSunPrimitive {
         #if UNITY_EDITOR
         private static Mesh CreateMesh() {

            Vector3[] vertices = new Vector3[41];

            float radius = 0.5f;
            float angle = 2 * Mathf.PI / 8f;

            for (int i = 0; i < 8; i++)
            {
                vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), radius * Mathf.Cos(i * angle), 0f);
            }

            radius = 0.3f;
            angle = 2 * Mathf.PI / 16f;
            for (int i = 0; i < 16; i++)
            {
                float a = 0.5f + i;
                vertices[8 + i] = new Vector3(radius * Mathf.Sin(a * angle), radius * Mathf.Cos(a * angle), 0f);
            }
            
            radius = 0.2f;

            for (int i = 0; i < 16; i++)
            {
                float a = i;
                vertices[24 + i] = new Vector3(radius * Mathf.Sin(a * angle), radius * Mathf.Cos(a * angle), 0f);
            }
            vertices[40] = new Vector3(0, 0, 0);

            Vector2[] uv = new Vector2[41];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(vertices[i].x, vertices[i].y);
            }


        int[] triangles = { 0,8,23,4,16,15,
            40,24,25,40,25,26,40,26,27,40,27,28,40,28,29,40,29,30,40,30,31,40,31,32,40,32,33,
            40,33,34,40,34,35,40,35,36,40,36,37,40,37,38,40,38,39,40,39,24};
     
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
             var obj = new GameObject("TwoPointSun");
             var mesh = CreateMesh();
             var filter = obj.AddComponent<MeshFilter>();
             var renderer = obj.AddComponent<MeshRenderer>();
             var collider = obj.AddComponent<MeshCollider>();
     
             filter.sharedMesh = mesh;
             collider.sharedMesh = mesh;
             renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");

        string path = "Assets/GeneratedMeshes/SubTwoPointMesh.asset";
        SaveAsset.SaveMeshAsset(mesh, path);

        return obj;
         }
     
         [MenuItem("GameObject/3D Object/TwoPointSun", false, 0)]
         public static void Create() {
             CreateObject();
         }
         #endif
     }