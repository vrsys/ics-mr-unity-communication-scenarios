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
    private GameObject objectSet0;
    
    [SerializeField]
    private GameObject objectSet1;

    [SerializeField]
    private GameObject objectSetStacked0;

    [SerializeField]
    private GameObject objectSetStacked1;

    [SerializeField]
    private GameObject doubledShapes;

    [SerializeField]
    private GameObject differentShapes;

    [SerializeField]
    private GameObject trainingScenario;

    public bool taskComplete;

    public class ShapeInfo
    {

        public string p0_shape { get; set; }

        public string p1_shape { get; set; }
    }

    // 'random' positions for differences (but same for each pair)
    private List<List<int>> objectIDsWithDifferencesPerTrial = new List<List<int>>() {
        new List<int>(){ 0,  1, 10},
        new List<int>(){ 5, 13,  9},
        new List<int>(){13,  2,  8},
        new List<int>(){ 7, 13, 12}
    };
    private List<List<int>> objectIDsWithoutDifferencesPerTrial = new List<List<int>>() {
        new List<int>(){ 8,9,5,2,4,6,13,7,3,12,11},
        new List<int>(){ 11,6,8,4,2,1,7,10,12,0,3},
        new List<int>(){ 11,12,6,1,0,9,7,4,10,3,5},
        new List<int>(){ 1,0,11,8,9,2,4,6,5,10,3}
    };

    private int numObjectsPerPerson;

    private List<GameObject> diffObjects0;
    private List<GameObject> diffObjects1;
    private List<GameObject> diffObjectsStacked0;
    private List<GameObject> diffObjectsStacked1;

    private List<bool> markerStates0;
    private List<bool> markerStates1;

    private List<CubeMarker> marker0;
    private List<CubeMarker> marker1;
    
    private int activeTrial;
    private bool shadowingActive;


    [SerializeField]
    private string resourceDirectory;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        HideBoxes(true);
        HideBoxes(false);
        //HideShapes();

        HideTrainingScenario();

        taskComplete = false;
    
        marker0 = new List<CubeMarker>();
        marker1 = new List<CubeMarker>();
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
        if (objectSet0 == null)
        {
            objectSet0 = GameObject.Find("ObjectSet0");
        }
        if (objectSet1 == null)
        {
            objectSet1 = GameObject.Find("ObjectSet1");
        }
        if (objectSetStacked0 == null)
        {
            objectSetStacked0 = GameObject.Find("ObjectSetStacked0");
        }
        if (objectSetStacked1 == null)
        {
            objectSetStacked1 = GameObject.Find("ObjectSetStacked1");
        }
        if (doubledShapes == null)
        {
            doubledShapes = GameObject.Find("DoubledShapes");
        }
        if (differentShapes == null)
        {
            differentShapes = GameObject.Find("DifferentShapes");
        }
        if (trainingScenario == null)
        {
            trainingScenario = GameObject.Find("TrainingScenario");
        }

        


        diffObjects0 = GetAllChildrenWithTag(objectSet0.transform, "DiffObject");
        diffObjects1 = GetAllChildrenWithTag(objectSet1.transform, "DiffObject");
        diffObjectsStacked0 = GetAllChildrenWithTag(objectSetStacked0.transform, "DiffObject");
        diffObjectsStacked1 = GetAllChildrenWithTag(objectSetStacked1.transform, "DiffObject");

        if (diffObjects0.Count != diffObjects1.Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }

        if (diffObjectsStacked0.Count != diffObjectsStacked1.Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }

        if (diffObjectsStacked0.Count != diffObjects0.Count)
        {
            Debug.LogError("Two sets of difference objects have different numbers of objects");
        }


        numObjectsPerPerson = diffObjects0.Count;

   
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

    public void ShowTrainingScenario()
    {
       trainingScenario.transform.position += new Vector3(0, 10, 0);

    }

    public void HideTrainingScenario()
    {
        trainingScenario.transform.position -= new Vector3(0, 10, 0);
    }

    public void InitializeBoxesAndShapesForTrialNew(int trial, bool shadowing)
    {
        taskComplete = false;

        activeTrial = trial;
        shadowingActive = shadowing;

        ShowBoxes();
        CreateShapesForBoxes();
        //InitMarkerStateArrays();
    }

    //public void InitializeBoxesAndShapesForTrial(int trial, bool shadowing)
    //{
    //    taskComplete = false;

    //    activeTrial = trial;
    //    shadowingActive = shadowing;

    //    ShowBoxes();
    //    AttachShapesToBoxes();
    //    InitMarkerStateArrays();
    //}

    public void HideBoxesAndShapes()
    {
        HideShapes();
        HideBoxes(shadowingActive);
    }

    private void InitMarkerStateArrays()
    {
        int numCubeMarkersPerParticipant = objectIDsWithoutDifferencesPerTrial[activeTrial].Count + objectIDsWithDifferencesPerTrial[activeTrial].Count;
        markerStates0 = new List<bool>( new bool[numCubeMarkersPerParticipant] );
        markerStates1 = new List<bool>( new bool[numCubeMarkersPerParticipant] );

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
            marker0[id].SetMarkerState(state);
        }
        else if (shapeSet == 1)
        {
            markerStates1[id] = state;
            marker1[id].SetMarkerState(state);
        }

        if (IsTaskComplete())
        {
            taskComplete = true;
        }
    }
    
    
    public void MarkerSet(bool state, int id, int shapeSet)
    {
        // TODO support remote marker setting
        //photonView.RPC("RemoteMarkerSet", RpcTarget.Others, state, id , shapeSet);

        LocalMarkerSet(state, id, shapeSet);
    }

    private bool IsTaskComplete()
    {


        //Debug.Log("Difference ids: ");
        //foreach (var item in objectIDsWithDifferencesPerTrial[activeTrial])
        //{
        //    Debug.Log(item);
        //}

        // check if only the objects with different shapes are marked

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


        return true;
    }




    private void HideBoxes(bool shadowing)
    {

        objectSetStacked0.SetActive(false);
        objectSetStacked1.SetActive(false);
        objectSet0.SetActive(false);
        objectSet1.SetActive(false);

        //if (shadowing)
        //{
        //    objectSetStacked0.transform.position += new Vector3(0, -10, 0);
        //    objectSetStacked1.transform.position += new Vector3(0, -10, 0);
        //}
        //else
        //{
        //    objectSet0.transform.position += new Vector3(0, -10, 0);
        //    objectSet1.transform.position += new Vector3(0, -10, 0);
        //}

    }
    private void ShowBoxes()
    {

        if (shadowingActive)
        {
            objectSetStacked0.SetActive(true);
            objectSetStacked1.SetActive(true);

            //objectSetStacked0.transform.position += new Vector3(0, 10, 0);
            //objectSetStacked1.transform.position += new Vector3(0, 10, 0);
        }
        else
        {
            objectSet0.SetActive(true);
            objectSet1.SetActive(true);
            //objectSet0.transform.position += new Vector3(0, 10, 0);
            //objectSet1.transform.position += new Vector3(0, 10, 0);
        }
    }

    private void HideShapes()
    {
        doubledShapes.transform.position += new Vector3(0, -10, 0);

        differentShapes.transform.Find("Trial" + activeTrial).transform.position += new Vector3(0, -10, 0);
    }


    private GameObject AttachShapeToBox(string shapeString, GameObject boxObject)
    {
        // split string by first capital letter
        string[] split = System.Text.RegularExpressions.Regex.Split(shapeString, @"(?<!^)(?=[A-Z])");
        string color = split[0];
        string shape = split[1];

        // instantiate prefab for shape
        GameObject shapeObject = Instantiate(Resources.Load<GameObject>(resourceDirectory + "Prefabs/Shapes/" + shape));

        // apply material
        Material material = Resources.Load<Material>(resourceDirectory + "Materials/" + color);

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

        string csvPath = resourceDirectory + "/ShapeLists/shape_lists_" + activeTrial;
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

                foreach (var item in shapeInfo)
                {
                    Debug.Log(item.p0_shape);
                    Debug.Log(item.p1_shape);
                }
            }

        }


        if (shapeInfo.Count != numObjectsPerPerson)
        {
            Debug.LogError("Number of shapes in csv file does not match number of objects");
        }


        List<GameObject> activeDiffObjects0;
        List<GameObject> activeDiffObjects1;

        if (shadowingActive)
        {
            activeDiffObjects0 = diffObjectsStacked0;
            activeDiffObjects1 = diffObjectsStacked1;
        }
        else
        {
            activeDiffObjects0 = diffObjects0;
            activeDiffObjects1 = diffObjects1;
        }

        List<GameObject> shapes = new List<GameObject>();

        for (int i = 0; i < numObjectsPerPerson; i++)
        {
            
            GameObject shapeObject0 = AttachShapeToBox(shapeInfo[i].p0_shape, activeDiffObjects0[i]);
            GameObject shapeObject1 = AttachShapeToBox(shapeInfo[i].p1_shape, activeDiffObjects1[i]);
            shapes.Add(shapeObject0);
            shapes.Add(shapeObject1);

        }




    }

    private void AttachShapesToBoxes()
    {
        if (activeTrial > objectIDsWithDifferencesPerTrial.Count - 1)
        {
            Debug.LogError("Trial number too large");
        }

        List<Transform> doubledShapeTransforms = GetAllChildren(doubledShapes.transform);
        List<Transform> differentShapeTransforms = GetAllChildren(differentShapes.transform.Find("Trial" + activeTrial).transform.transform);


        int doubledShapesPlaced = 0;
        int differentShapesPlaced = 0;

        List<int> objectIDsWithDifferences = objectIDsWithDifferencesPerTrial[activeTrial];

        List<GameObject> activeDiffObjects0;
        List<GameObject> activeDiffObjects1;
        
        if (shadowingActive)
        {
            activeDiffObjects0 = diffObjectsStacked0;
            activeDiffObjects1 = diffObjectsStacked1;
        }
        else
        {
            activeDiffObjects0 = diffObjects0;
            activeDiffObjects1 = diffObjects1;
        }

        marker0.Clear();
        marker1.Clear();

        for (int i = 0; i < numObjectsPerPerson; i++)
        {
            if (objectIDsWithDifferences.Contains(i))
            {
                int target_object_for_shape = objectIDsWithDifferencesPerTrial[activeTrial][differentShapesPlaced];

                differentShapeTransforms[differentShapesPlaced * 2].position = activeDiffObjects0[target_object_for_shape].transform.position;
                differentShapeTransforms[differentShapesPlaced * 2].rotation = activeDiffObjects0[target_object_for_shape].transform.rotation;

                differentShapeTransforms[differentShapesPlaced * 2 + 1].position = activeDiffObjects1[target_object_for_shape].transform.position;
                differentShapeTransforms[differentShapesPlaced * 2 + 1].rotation = activeDiffObjects1[target_object_for_shape].transform.rotation;

                ++differentShapesPlaced;
            }
            else
            {
                int target_object_for_shape = objectIDsWithoutDifferencesPerTrial[activeTrial][doubledShapesPlaced];

                doubledShapeTransforms[doubledShapesPlaced * 2].position = activeDiffObjects0[target_object_for_shape].transform.position;
                doubledShapeTransforms[doubledShapesPlaced * 2].rotation = activeDiffObjects0[target_object_for_shape].transform.rotation;

                doubledShapeTransforms[doubledShapesPlaced * 2 + 1].position = activeDiffObjects1[target_object_for_shape].transform.position;
                doubledShapeTransforms[doubledShapesPlaced * 2 + 1].rotation = activeDiffObjects1[target_object_for_shape].transform.rotation;

                ++doubledShapesPlaced;

            }

            bool isDifference = false;
            if(objectIDsWithDifferencesPerTrial[activeTrial].Contains(i))
                isDifference = true;
            
            // set cube ID to enable marker to report which cube has been selected, and give reference 
            CubeMarker cubeMarker0 = activeDiffObjects0[i].transform.GetComponent<CubeMarker>();
            cubeMarker0.id = i;
            cubeMarker0.diffObjectsController = this;
            cubeMarker0.shapeSet = 0;
            cubeMarker0.isDifferenceCube = isDifference;
            marker0.Add(cubeMarker0);
            
            CubeMarker cubeMarker1 = activeDiffObjects1[i].transform.GetComponent<CubeMarker>();
            cubeMarker1.id = i;
            cubeMarker1.diffObjectsController = this;
            cubeMarker1.shapeSet = 1;
            cubeMarker1.isDifferenceCube = isDifference;
            marker1.Add(cubeMarker1);
        }
    }
}
