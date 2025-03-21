     #if UNITY_EDITOR
     using UnityEngine;
     using UnityEditor;
     #endif
     
     public class EvenCrossPrimitive {
         #if UNITY_EDITOR
         private static Mesh CreateMesh() {
         Vector3[] vertices = new Vector3[24];
        vertices[0] = new Vector3(-0.1f, 0.1f, 0f);
        vertices[1] = new Vector3(0.1f, 0.1f, 0f);
        vertices[2] = new Vector3(0.1f, -0.1f, 0f);
        vertices[3] = new Vector3(-0.1f, -0.1f, 0f);

        vertices[4] = new Vector3(-0.1f, 0.4f, 0f);
        vertices[5] = new Vector3(0.1f, 0.4f, 0f);
        vertices[6] = new Vector3(0.4f, 0.1f, 0f);
        vertices[7] = new Vector3(0.4f, -0.1f, 0f);

        vertices[8] = new Vector3(0.1f, -0.4f, 0f);
        vertices[9] = new Vector3(-0.1f, -0.4f, 0f);
        vertices[10] = new Vector3(-0.4f, -0.1f, 0f);
        vertices[11] = new Vector3(-0.4f, 0.1f, 0f);

        vertices[12] = new Vector3(-0.2f, 0.4f, 0f);
        vertices[13] = new Vector3(0f, 0.5f, 0f);
        vertices[14] = new Vector3(0.2f, 0.4f, 0f);

        vertices[15] = new Vector3(0.4f, 0.2f, 0f);
        vertices[16] = new Vector3(0.5f, 0f, 0f);
        vertices[17] = new Vector3(0.4f, -0.2f, 0f);

        vertices[18] = new Vector3(0.2f, -0.4f, 0f);
        vertices[19] = new Vector3(0f, -0.5f, 0f);
        vertices[20] = new Vector3(-0.2f, -0.4f, 0f);

        vertices[21] = new Vector3(-0.4f, -0.2f, 0f);
        vertices[22] = new Vector3(-0.5f, 0f, 0f);
        vertices[23] = new Vector3(-0.4f, 0.2f, 0f);

        Vector2[] uv = new Vector2[24];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].y);
        }


             
             int[] triangles = { 11,6,7,11,7,10,4,5,8,4,8,9, 12,13,14,15,16,17,18,19,20,21,22,23};

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
             var obj = new GameObject("EvenCross");
             var mesh = CreateMesh();
             var filter = obj.AddComponent<MeshFilter>();
             var renderer = obj.AddComponent<MeshRenderer>();
             var collider = obj.AddComponent<MeshCollider>();
     
             filter.sharedMesh = mesh;
             collider.sharedMesh = mesh;
             renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
     
             return obj;
         }
     
         [MenuItem("GameObject/3D Object/EvenCross", false, 0)]
         public static void Create() {
             CreateObject();
         }
         #endif
     }