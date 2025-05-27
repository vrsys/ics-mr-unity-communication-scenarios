using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MaterialCreator : EditorWindow
{

    private List<string> textureFolders = new List<string>
    {
        "Assets\\ics-mr\\Resources\\ics-mr-communication-scenario-materials\\S1_FloorPlanNegotiation\\FloorPlanImages\\4Rooms",
        "Assets\\ics-mr\\Resources\\ics-mr-communication-scenario-materials\\S1_FloorPlanNegotiation\\FloorPlanImages\\5Rooms",
        "Assets\\ics-mr\\Resources\\ics-mr-communication-scenario-materials\\S3_SurvivalGame\\SurvivalItemImages"
    };

    private List<string> materialFolders = new List<string>
    {
        "Assets\\ics-mr\\Resources\\S1_FloorPlanNegotiation\\FloorPlanMaterials\\4Rooms",
        "Assets\\ics-mr\\Resources\\S1_FloorPlanNegotiation\\FloorPlanMaterials\\5Rooms",
        "Assets\\ics-mr\\Resources\\S3_SurvivalGame\\Materials\\SurvivalItemImageMaterials"
    };

    [MenuItem("Tools/Create Materials From Textures")]
    public static void ShowWindow()
    {
        GetWindow<MaterialCreator>("Material Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Creation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Materials"))
        {
            CreateMaterials();
        }
    }

    private void CreateMaterials()
    {
        if (textureFolders.Count != materialFolders.Count)
        {
            Debug.LogError("Texture and Material folder counts do not match.");
            return;
        }

        for (int i = 0; i < textureFolders.Count; i++)
        {
            string texturesFolderPath = textureFolders[i];
            string materialsFolderPath = materialFolders[i];

            if (!Directory.Exists(texturesFolderPath))
            {
                Debug.LogError("Textures folder not found: " + texturesFolderPath);
                return;
            }

            if (!AssetDatabase.IsValidFolder(materialsFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }

            string[] textureFiles = Directory.GetFiles(texturesFolderPath, "*.png");

            foreach (string file in textureFiles)
            {
                string assetPath = file.Replace(Application.dataPath, "Assets"); // Convert to relative path
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                if (texture != null)
                {
                    Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                    newMaterial.mainTexture = texture;

                    string materialPath = Path.Combine(materialsFolderPath, texture.name + ".mat");
                    AssetDatabase.CreateAsset(newMaterial, materialPath);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Materials created successfully.");
    }
}
