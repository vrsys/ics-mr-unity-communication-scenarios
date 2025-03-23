using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialCreator : EditorWindow
{
    private string texturesFolderPath = "Assets/aplause-mr/Resources/TASK3/Materials/SurvivalItemImageMaterials"; // Change to your directory
    private string materialsFolderPath = "Assets/aplause-mr/Resources/TASK3/Materials/SurvivalItemImageMaterials";

    [MenuItem("Tools/Create Materials From Textures")]
    public static void ShowWindow()
    {
        GetWindow<MaterialCreator>("Material Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Creation", EditorStyles.boldLabel);
        texturesFolderPath = EditorGUILayout.TextField("Textures Folder", texturesFolderPath);
        materialsFolderPath = EditorGUILayout.TextField("Materials Folder", materialsFolderPath);

        if (GUILayout.Button("Generate Materials"))
        {
            CreateMaterials();
        }
    }

    private void CreateMaterials()
    {
        if (!Directory.Exists(texturesFolderPath))
        {
            Debug.LogError("Textures folder not found: " + texturesFolderPath);
            return;
        }

        if (!AssetDatabase.IsValidFolder(materialsFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string[] textureFiles = Directory.GetFiles(texturesFolderPath, "*.png"); // Change to match your image format

        foreach (string file in textureFiles)
        {
            string assetPath = file.Replace(Application.dataPath, "Assets"); // Convert to relative path
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture != null)
            {
                //Material newMaterial = new Material(Shader.Find("Standard"));
                Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                newMaterial.mainTexture = texture;

                string materialPath = Path.Combine(materialsFolderPath, texture.name + ".mat");
                AssetDatabase.CreateAsset(newMaterial, materialPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Materials created successfully.");
    }
}
