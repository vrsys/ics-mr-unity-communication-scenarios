using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SaveAsset
{
    public static void SaveMeshAsset(Mesh mesh, string path)
    {
        // Ensure the directory exists
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Save the mesh asset
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
#endif