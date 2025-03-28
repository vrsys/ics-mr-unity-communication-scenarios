using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using CsvHelper;

public class DifferenceObjectController : MonoBehaviour
{
    [SerializeField]
    private GameObject boxSetSpreadRoot0;
    
    [SerializeField]
    private GameObject boxSetSpreadRoot1;

    [SerializeField]
    private GameObject boxSetStackedRoot0;

    [SerializeField]
    private GameObject boxSetStackedRoot1;
    

    [SerializeField]
    private GameObject boxSetTrainingRoot0;

    [SerializeField]
    private GameObject boxSetTrainingRoot1;

    //private bool taskComplete = false;

    public class ShapeInfo
    {

        public string p0_col { get; set; }
        public string p0_shape { get; set; }

        public string p1_col { get; set; }
        public string p1_shape { get; set; }
    }



    private List<bool> markerStates0;
    private List<bool> markerStates1;

    private List<CubeMarker> markers0;
    private List<CubeMarker> markers1;
    
    private int activeTrial;

    private List<GameObject> shapes = new List<GameObject>();


    private GameObject activeBoxesRoot0;
    private GameObject activeBoxesRoot1;

    [SerializeField]
    private string resourceDirectory;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        boxSetStackedRoot0.SetActive(false);
        boxSetStackedRoot1.SetActive(false);
        boxSetSpreadRoot0.SetActive(false);
        boxSetSpreadRoot1.SetActive(false);
        boxSetTrainingRoot0.SetActive(false);
        boxSetTrainingRoot1.SetActive(false);

    
        markers0 = new List<CubeMarker>();
        markers1 = new List<CubeMarker>();
    }


    public static List<GameObject> GetAllChildrenWithTag(Transform parent, string tag)
    {
        List<GameObject> foundObjects = new List<GameObject>();

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) // true = include inactive objects
        {
            if (child.CompareTag(tag)) // Check if the child's tag matches
            {
                foundObjects.Add(child.gameObject);
            }
        }

        return foundObjects;
    }

    void Init() {

        if (GetAllChildrenWithTag(boxSetSpreadRoot0.transform, "DiffObject").Count != GetAllChildrenWithTag(boxSetSpreadRoot1.transform, "DiffObject").Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }
        if (GetAllChildrenWithTag(boxSetStackedRoot0.transform, "DiffObject").Count != GetAllChildrenWithTag(boxSetStackedRoot1.transform, "DiffObject").Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }
        if (GetAllChildrenWithTag(boxSetTrainingRoot0.transform, "DiffObject").Count != GetAllChildrenWithTag(boxSetTrainingRoot1.transform, "DiffObject").Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }


   
    }

    private List<Transform> GetAllChildren(Transform parent)
    {
        List<Transform> transformList = new List<Transform>();
        foreach (Transform child in parent)
        {
            transformList.Add(child);
        }
        return transformList;
    }



    public void InitializeBoxesAndShapesForTrial(int trial, bool _showStackedBoxes)
    {
        //taskComplete = false;

        activeTrial = trial;

        if (trial < 0)
        {
            activeBoxesRoot0 = boxSetTrainingRoot0;
            activeBoxesRoot1 = boxSetTrainingRoot1;

        }
        else if (_showStackedBoxes)
        {
            activeBoxesRoot0 = boxSetStackedRoot0;
            activeBoxesRoot1 = boxSetStackedRoot1;
        }
        else
        {
            activeBoxesRoot0 = boxSetSpreadRoot0;
            activeBoxesRoot1 = boxSetSpreadRoot1;
        }

        ShowBoxes();
        CreateShapesForBoxes();
    }


    public void HideBoxesAndShapes()
    {
        HideBoxes();

        foreach (var shape in shapes)
        {
            Destroy(shape);
        }
        shapes.Clear();
    }


    void RemoteMarkerSet(bool state, int id, int shapeSet)
    {
        Debug.Log("Received remote marker set call. Set: " + shapeSet + ", id: " + id + ", state: " + state);
        
        LocalMarkerSet(state, id, shapeSet);
    }


    void LocalMarkerSet(bool state, int id, int shapeSet)
    {
        Debug.Log("Received local marker set call. Set: " + shapeSet + ", id: " + id + ", state: " + state);
        
        if (shapeSet == 0)
        {
            markerStates0[id] = state;
        }
        else if (shapeSet == 1)
        {
            markerStates1[id] = state;
        }
        
        /*
        if (IsTaskComplete())
        {
            taskComplete = true;
        }
        */
    }
    
    
    public void MarkerSet(bool state, int id, int shapeSet)
    {
        // TODO support remote marker setting
        //photonView.RPC("RemoteMarkerSet", RpcTarget.Others, state, id , shapeSet);

        LocalMarkerSet(state, id, shapeSet);
    }

    private bool IsTaskComplete()
    {
        /*

        int numDiffObjects = objectIDsWithoutDifferencesPerTrial[activeTrial].Count + objectIDsWithDifferencesPerTrial[activeTrial].Count;
        List<int> diffLocations = objectIDsWithDifferencesPerTrial[activeTrial];

        // check for any errors in marker state
        for (int i = 0; i < numDiffObjects; i++)
        {
            // if should be a difference at this ID
            if (diffLocations.Contains(i))
            {
                //if (markerStates0[i] == false)
                if (markerStates0[i] == false || markerStates1[i] == false)
                {
                    return false;
                }
            }
            // if should NOT be a difference at this ID
            else
            {
                //if (markerStates0[i] == true)
                if (markerStates0[i] == true || markerStates1[i] == true)
                {
                    return false;
                }
            }
        }

        */
        return true;
    }




    private void HideBoxes()
    {

        if (activeBoxesRoot0 != null)
        {
            activeBoxesRoot0.SetActive(false);
        }
        if (activeBoxesRoot1 != null)
        {
            activeBoxesRoot1.SetActive(false);
        }


    }
    private void ShowBoxes()
    {
        activeBoxesRoot0.SetActive(true);
        activeBoxesRoot1.SetActive(true);
    }


    private GameObject AttachShapeToBox(string color, string shape, GameObject boxObject)
    {
        // instantiate prefab for shape
        string prefabPath = resourceDirectory + "/Prefabs/Shapes/" + shape;
        GameObject prefabToInstantiate = Resources.Load<GameObject>(prefabPath);
        if (prefabToInstantiate == null)
        {
            Debug.LogError("Could not load prefab: " + prefabPath);
        }
        GameObject shapeObject = Instantiate(prefabToInstantiate);

        // apply material
        string materialPath = resourceDirectory + "/Materials/" + color.ToLower() + "_mat";
        Material material = Resources.Load<Material>(materialPath);
        if (material == null)
        {
            Debug.LogError("Could not load material: " + materialPath);
        }

        // find all renderers in children of shapeObject
        Renderer[] renderers = shapeObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }

        // set transform
        shapeObject.transform.position = boxObject.transform.position;
        shapeObject.transform.rotation = boxObject.transform.rotation;

        return shapeObject;
    }

    private void CreateShapesForBoxes()
    {

        string csvPath;
        if (activeTrial < 0)
        {
            csvPath = resourceDirectory + "/ShapeLists/shape_lists_training";
        }
        else
        {
            csvPath = resourceDirectory + "/ShapeLists/shape_lists_" + activeTrial;
        }
        TextAsset csvFile = Resources.Load<TextAsset>(csvPath);
        if (csvFile == null)
        {
            Debug.LogError("Could not load csv file: " + csvPath);
            return;
        }
        else
        {
            Debug.Log("Loaded csv file: " + csvPath);
        }

        List<ShapeInfo> shapeInfo;

        using (var reader = new StringReader(csvFile.text))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                shapeInfo = new List<ShapeInfo>(csv.GetRecords<ShapeInfo>());
            }

        }


        int numDifferences = 0;
        for (int i = 0; i < shapeInfo.Count; i++)
        {
            if (shapeInfo[i].p0_col != shapeInfo[i].p1_col || shapeInfo[i].p0_shape != shapeInfo[i].p1_shape)
            {
                ++numDifferences;
            }
        }

        List<GameObject> activeBoxes0 = GetAllChildrenWithTag(activeBoxesRoot0.transform, "DiffObject");
        List<GameObject> activeBoxes1 = GetAllChildrenWithTag(activeBoxesRoot1.transform, "DiffObject");


        if (shapeInfo.Count != activeBoxes0.Count)
        {
            Debug.LogError("Number of shapes in csv file does not match number of objects: " + shapeInfo.Count + "/" + activeBoxes0.Count);
        }

        shapes = new List<GameObject>();

        for (int i = 0; i < activeBoxes0.Count; i++)
        {
            
            GameObject shapeObject0 = AttachShapeToBox(shapeInfo[i].p0_col, shapeInfo[i].p0_shape, activeBoxes0[i]);
            GameObject shapeObject1 = AttachShapeToBox(shapeInfo[i].p1_col, shapeInfo[i].p1_shape, activeBoxes1[i]);

            shapeObject0.name = shapeInfo[i].p0_col + shapeInfo[i].p0_shape + "_0";
            shapeObject1.name = shapeInfo[i].p1_col + shapeInfo[i].p1_shape + "_1";

            shapes.Add(shapeObject0);
            shapes.Add(shapeObject1);

        }


    }

    private void ResetCubeMarkers(List<GameObject> activeBoxes0, List<GameObject> activeBoxes1)
    {


        markerStates0 = new List<bool>( new bool[activeBoxes0.Count] );
        markerStates1 = new List<bool>( new bool[activeBoxes0.Count] );

        if (null != markers0)
        {
            markers0.Clear();
        }
        if (null != markers1)
        {
            markers1.Clear();
        }


        for (int i = 0; i < activeBoxes0.Count; i++)
        {
            // set cube ID to enable marker to report which cube has been selected, and give reference 
            CubeMarker cubeMarker0 = activeBoxes0[i].transform.GetComponent<CubeMarker>();
            cubeMarker0.ResetMarker(i, 0);
            markers0.Add(cubeMarker0);

            CubeMarker cubeMarker1 = activeBoxes1[i].transform.GetComponent<CubeMarker>();
            cubeMarker0.ResetMarker(i, 1);
            markers1.Add(cubeMarker1);
        }
    }


    private void Update()
    {
        // TODO check if task is complete?
    }
}
