using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class SurvivalItemController : MonoBehaviour
{

    [SerializeField]
    private float distFromCentre = 0.7f;
    [SerializeField]
    private float threeLinesGroupSpan = 1.5f;
    [SerializeField]
    private GameObject survivalItemPrefab;
    [SerializeField]
    private string taskDataResourceDirectory = "SurvivalTask";
    [SerializeField]
    private int numSurvivalItems = 15;



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

    public enum SurvivalScenario
    {
        DESERT,
        SEA,
        WINTER,
        MOUNTAINS
    }

    [SerializeField]
    private List<SurvivalScenario> survivalScenarioOrder = new List<SurvivalScenario> { SurvivalScenario.MOUNTAINS, SurvivalScenario.WINTER, SurvivalScenario.DESERT, SurvivalScenario.SEA };


/*
public override void OnJoinedRoom()
{
    InstantiateSurvivalItemsIfMaster();
}

private void InstantiateSurvivalItemsIfMaster()
{
    if (PhotonNetwork.IsMasterClient)
    {
        string prefabPath = prefabResourceDirectory + "/" + survivalItemPrefab.name;

        for (int i = 0; i < numSurvivalItems; i++)
        {
            GameObject newGameObject = PhotonNetwork.InstantiateSceneObject(prefabPath, Vector3.zero, Quaternion.identity);
            newGameObject.name = survivalItemPrefab.name + newGameObject.GetComponent<PhotonView>().ViewID;
            //newGameObject.name = survivalItemPrefab.name + i;
            newGameObject.transform.SetParent(transform, false);

            //furnitureGameObject.transform.localPosition = Vector3.zero;
        }

        InitialiseReferences();
    }
}


public void InitialiseSurvivalItemsIfClient()
{
    if (!PhotonNetwork.IsMasterClient)
    {
        for (int i = 0; i < numSurvivalItems; i++)
        {
            GameObject itemGameObject = GameObject.Find(survivalItemPrefab.name + "(Clone)");
            if (itemGameObject != null)
            {
                itemGameObject.name = survivalItemPrefab.name + itemGameObject.GetComponent<PhotonView>().ViewID;
                itemGameObject.transform.SetParent(transform, false);
                //itemGameObject.transform.localPosition = Vector3.zero;
            }
        }

        InitialiseReferences();
    }
}

private void InitialiseReferences()
{
    foreach (Transform child in transform)
    {
        itemGameObjects.Add(child.gameObject);

    }
    // order by viewID to make sure that all clients have the same order
    itemGameObjects = itemGameObjects.OrderBy(itemGameObj => itemGameObj.GetComponent<PhotonView>().ViewID).ToList();
}
*/

    public void ShowBoxesForSurvivalScenario(int scenarioIndex)
    {
        SurvivalScenario survivalScenario = survivalScenarioOrder[scenarioIndex];
        string csvPath = taskDataResourceDirectory + "/SurvivalItemData/" + survivalScenario.ToString() + "_survival_items";

        DisplayItemsFromCSV(csvPath);
    }

    public void DisplayItemsFromCSV(string csvPath)
    {
        LoadItemsFromCSV(csvPath);
        ApplyItemLabelsAndImages();
        PositionItems();
    }
    public void DisplayItemsFromCSV(string csvPath, int randomSeed)
    {
        LoadItemsFromCSV(csvPath);
        //RandomizeItemOrderWithSeed(randomSeed);
        RandomizeItemOrderWithinGroupsWithSeed(randomSeed, 3);
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

    private void RandomizeItemOrderWithSeed(int randomSeed)
    {
        Random.seed = randomSeed;
        items = items.OrderBy(x => Random.value).ToList();
    }

    private void RandomizeItemOrderWithinGroupsWithSeed(int randomSeed, int numGroups)
    {
        Random.seed = randomSeed;
        int itemsPerGroup = items.Count / numGroups;

        List<ItemInfo> randomizedItemList = new List<ItemInfo>();

        // iterate through groups to randomize items within each group
        for (int i = 0; i < numGroups; ++i)
        {
            List<ItemInfo> groupItems = items.GetRange(i * itemsPerGroup, itemsPerGroup);
            groupItems = groupItems.OrderBy(x => Random.value).ToList();
            randomizedItemList.AddRange(groupItems);
        }

        items = randomizedItemList;
    }


    private void ApplyItemLabelsAndImages()
    {
        string materialDirectory = taskDataResourceDirectory + "/Materials";


        for (int i = 0; i < items.Count; i++)
        {
            //Debug.Log(i + ", " + items[i].longName);

            //itemGameObjects[i].transform.Find("Canvas").transform.Find("Text").GetComponent<TextMeshProUGUI>().text = items[i].longName;
            itemGameObjects[i].GetComponent<ItemCube>().textField.SetText(items[i].longName);
            itemGameObjects[i].name = "Item_" + items[i].shortName;

            Material itemMaterial = (Material)Resources.Load("SurvivalTask/Materials/" + items[i].id, typeof(Material));
            itemGameObjects[i].GetComponent<ItemCube>().imageRenderer.material = itemMaterial;
        }
    }

    private void PositionItems()
    {
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        foreach (GameObject obj in itemGameObjects)
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();
            photonView.RequestOwnership();
        }
        */

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
            // TODO set inactive
            itemGameObjects[i].transform.localPosition = new Vector3(0,-1,0);
        }

    }

    public void HideItems()
    {
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        */

        foreach (GameObject obj in itemGameObjects)
        {
            //PhotonView photonView = obj.GetComponent<PhotonView>();
            //photonView.RequestOwnership();

            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.position = new Vector3(0f, -1f, 0f);
        }
    }

    /*
    // partial implementation of IPunOwnershipCallbacks to react object is transferred to another player
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        // set transferred object to kinematic, so gravity is disabled (the new owner will handle gravity)
        GameObject obj = targetView.gameObject;
        obj.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
    */
}
