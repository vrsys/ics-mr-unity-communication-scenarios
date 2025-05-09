using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using VRSYS.Core.Interaction;

public class SurvivalItemController : NetworkBehaviour
{

    [SerializeField]
    private float distFromCentre = 0.7f;
    [SerializeField]
    private float threeLinesGroupSpan = 1.5f;
    [SerializeField]
    private GameObject survivalItemPrefab;

    [SerializeField]
    private string unityResourceDirectory = "TASK3";

    private int maxNumSurvivalItems = 15;



    private class ItemInfo
    {
        public string id { get; set; }
        public string longName { get; set; }
        public string shortName { get; set; }

        public void print()
        {
            Debug.Log(id + ", " + longName + ", " + shortName);
        }
    }

    private List<ItemInfo> items = new List<ItemInfo>();

    private List<GameObject> itemGameObjects = new List<GameObject>();
    private enum ItemLayout
    {
        THREE_GROUPS,
        CIRCLE,
        THREE_LINES
    };
    [SerializeField]
    private ItemLayout itemLayout = ItemLayout.CIRCLE;


    public void CreateObjects()
    {
        // networked mode
        if (NetworkManager.Singleton)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                InstantiateSurvivalItemsIfMaster();
            }
        }
        // non networked mode
        else
        {
            InstantiateSurvivalItemsIfMaster();
        }

    }

    private void InstantiateSurvivalItemsIfMaster()
    {

        for (int i = 0; i < maxNumSurvivalItems; i++)
        {
            GameObject newGameObject = Instantiate(survivalItemPrefab);

            // spawn networked object if in networked mode
            if (NetworkManager.Singleton)
            {
                var instanceNetworkObject = newGameObject.GetComponent<NetworkObject>();
                instanceNetworkObject.Spawn();
            }


            newGameObject.name = survivalItemPrefab.name + i.ToString("000");
            newGameObject.transform.SetParent(transform, false);
            newGameObject.transform.localPosition = new Vector3(0, -1, 0);

        }

    }

    private void InitialiseReferences()
    {
        itemGameObjects = new List<GameObject>();

        foreach (Transform child in transform)
        {
            itemGameObjects.Add(child.gameObject);
        }

        if (NetworkManager.Singleton)
        {
            // order by network object id to make sure that all clients have the same order
            itemGameObjects = itemGameObjects.OrderBy(itemGameObj => itemGameObj.GetComponent<NetworkObject>().NetworkObjectId ).ToList();
        }
    }

    public void ShowBoxesForSurvivalScenario(string csvPath, int maxItemsToShow)
    {
        if (0 == itemGameObjects.Count)
        {
            InitialiseReferences();
        }

        
        LoadItemsFromCSV(csvPath);

        items = items.Take(maxItemsToShow).ToList();

        ApplyItemLabelsAndImages();

        PositionItems();
    }


    private void LoadItemsFromCSV(string filepath)
    {

        TextAsset csvFile = Resources.Load<TextAsset>(filepath);
        if (csvFile == null)
        {
            Debug.LogError("Could not load csv file: " + filepath);
            return;
        }
        else
        {
            Debug.Log("Loaded csv file: " + filepath);
        }

        items.Clear();

        using (var reader = new StringReader(csvFile.text))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                items = new List<ItemInfo>(csv.GetRecords<ItemInfo>());
            }

        }

    }



    private void ApplyItemLabelsAndImages()
    {
        string materialDirectory = unityResourceDirectory + "/Materials/SurvivalItemImageMaterials/";

        for (int i = 0; i < items.Count; i++)
        {
            itemGameObjects[i].GetComponent<ItemCube>().textField.SetText(items[i].longName);
            itemGameObjects[i].name = "Item_" + items[i].shortName;

            Material itemMaterial = (Material)Resources.Load(materialDirectory + items[i].id, typeof(Material));

            if (itemMaterial == null)
            {
                Debug.LogError("Could not find material: " + materialDirectory + items[i].id);
            }

            itemGameObjects[i].GetComponent<ItemCube>().imageRenderer.material = itemMaterial;
        }
    }

    private void PositionItems()
    {

        // in networked mode...
        if (NetworkManager.Singleton)
        {
            foreach (GameObject obj in itemGameObjects)
            {
                obj.GetComponent<NetworkRigidbody>().AutoUpdateKinematicState = true;
            }


            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            foreach (GameObject obj in itemGameObjects)
            {

                if (!obj.GetComponent<NetworkBehaviour>().IsOwner)
                {
                    obj.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.LocalClientId);
                }
            }
        }


        if (ItemLayout.CIRCLE == itemLayout)
        {
            float angleBetweenItems = (2f * Mathf.PI) / items.Count;

            for (int i = 0; i < items.Count; i++)
            {
                float a = angleBetweenItems * i;
                float xShift = distFromCentre * Mathf.Sin(a);
                float zShift = distFromCentre * Mathf.Cos(a);

                itemGameObjects[i].transform.localPosition = new Vector3(xShift, 0f, zShift);

                itemGameObjects[i].transform.localRotation = Quaternion.Euler(0f, (Mathf.Rad2Deg * a) + 180f, 0f);
            }
        }
        else if (ItemLayout.THREE_GROUPS == itemLayout)
        {
            int numGroups = 3;
            int itemsPerGroup = items.Count / numGroups;
            float angleBetweenGroupStart = (2f * Mathf.PI) / numGroups;
            float angleBetweenItemsInGroup = Mathf.Deg2Rad * 20f;

            for (int i = 0; i < items.Count; i++)
            {
                int group = i / itemsPerGroup;
                int withinGroupId = i % itemsPerGroup;

                float a = angleBetweenGroupStart * group + angleBetweenItemsInGroup * withinGroupId;
                float xShift = distFromCentre * Mathf.Sin(a);
                float zShift = distFromCentre * Mathf.Cos(a);

                itemGameObjects[i].transform.localPosition = new Vector3(xShift, 0f, zShift);
                itemGameObjects[i].transform.localRotation = Quaternion.Euler(0f, (Mathf.Rad2Deg * a) + 180f, 0f);
            }
        }
        else if (ItemLayout.THREE_LINES == itemLayout)
        {
            int numGroups = 3;
            int itemsPerGroup = items.Count / numGroups;

            float distanceBetweenItemsInGroup = 0;
            if (itemsPerGroup > 1)
            {
                distanceBetweenItemsInGroup = threeLinesGroupSpan / (itemsPerGroup - 1);
            }

            for (int i = 0; i < items.Count; i++)
            {
                int group = i / itemsPerGroup;
                int withinGroupId = i % itemsPerGroup;

                float zShift = distFromCentre;
                float xShift = -threeLinesGroupSpan / 2f + withinGroupId * distanceBetweenItemsInGroup;
                
                Vector3 v = new Vector3(xShift, 0f, zShift);
                itemGameObjects[i].transform.localPosition = Quaternion.Euler(0f, -group * 90f, 0f) * v;
                itemGameObjects[i].transform.localRotation = Quaternion.Euler(0f, -group * 90f, 0f);
            }
        }

        // make sure remaining cubes are hidden
        for (int i = items.Count; i < itemGameObjects.Count; i++)
        {
            itemGameObjects[i].transform.position = new Vector3(0,-1,0);
        }

    }

    public void HideItems()
    {
        if (0 == itemGameObjects.Count)
        {
            InitialiseReferences();
        }

        // in networked mode
        if (NetworkManager.Singleton)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            foreach (GameObject obj in itemGameObjects)
            {
                if (!obj.GetComponent<NetworkBehaviour>().IsOwner)
                {
                    obj.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.LocalClientId);
                }
            }

        }

        foreach (GameObject obj in itemGameObjects)
        {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.position = new Vector3(0, -1, 0);
        }
    }


}
