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
using Unity.Netcode;

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

    private APMR_ExperimentRunner experimentRunner;

    public class ShapeInfo
    {

        public string p0_col { get; set; }
        public string p0_shape { get; set; }

        public string p1_col { get; set; }
        public string p1_shape { get; set; }
    }

    // state of markers represented as bits in an integer
    private int markerStates0 = 0;
    private int markerStates1 = 0;
    private int targetMarkerState = 0;

    private int activeTrial;

    private List<GameObject> shapes = new List<GameObject>();


    private GameObject activeBoxesRoot0;
    private GameObject activeBoxesRoot1;

    [SerializeField]
    private string unityResourceDirectory;
    [SerializeField]
    private string dataResourceDirectory;

    // Start is called before the first frame update
    void Start()
    {
        experimentRunner = FindAnyObjectByType<APMR_ExperimentRunner>();
    }


    // preserves hierarchy order when getting children with tag
    public static List<GameObject> GetAllChildrenWithTag(Transform parent, string tag)
    {
        List<GameObject> foundObjects = new List<GameObject>();

        // Loop through children in the order they appear in the Hierarchy
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i); // Get child by index (preserves order)

            if (child.CompareTag(tag)) // Check if the child's tag matches
            {
                foundObjects.Add(child.gameObject);
            }

            // Recursively check the child's children while maintaining order
            foundObjects.AddRange(GetAllChildrenWithTag(child, tag));
        }

        return foundObjects;
    }

    public void Initialize() {

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

        HideAllBoxes();


    }

    public void InitializeBoxesAndShapesForTrial(int trial, bool _showStackedBoxes)
    {
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
        HideActiveBoxes();

        foreach (var shape in shapes)
        {
            Destroy(shape);
        }
        shapes.Clear();
    }

    private void HideAllBoxes()
    {
        boxSetStackedRoot0.SetActive(false);
        boxSetStackedRoot1.SetActive(false);
        boxSetSpreadRoot0.SetActive(false);
        boxSetSpreadRoot1.SetActive(false);
        boxSetTrainingRoot0.SetActive(false);
        boxSetTrainingRoot1.SetActive(false);
    }

    private void HideActiveBoxes()
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
        string prefabPath = unityResourceDirectory + "/Prefabs/Shapes/" + shape;
        GameObject prefabToInstantiate = Resources.Load<GameObject>(prefabPath);
        if (prefabToInstantiate == null)
        {
            Debug.LogError("Could not load prefab: " + prefabPath);
        }
        GameObject shapeObject = Instantiate(prefabToInstantiate);

        // apply material
        string materialPath = unityResourceDirectory + "/Materials/" + color.ToLower() + "_mat";
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
            csvPath = dataResourceDirectory + "/ShapeLists/shape_lists_training";
        }
        else
        {
            csvPath = dataResourceDirectory + "/ShapeLists/shape_lists_" + activeTrial;
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

        // count differences and get target marker state
        targetMarkerState = 0;

        int numDifferences = 0;
        for (int i = 0; i < shapeInfo.Count; i++)
        {
            if (shapeInfo[i].p0_col != shapeInfo[i].p1_col || shapeInfo[i].p0_shape != shapeInfo[i].p1_shape)
            {
                ++numDifferences;
                targetMarkerState |= (1 << i);
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

        ResetCubeMarkers(activeBoxes0, activeBoxes1);

    }

    private void ResetCubeMarkers(List<GameObject> activeBoxes0, List<GameObject> activeBoxes1)
    {


        markerStates0 = 0; 
        markerStates1 = 0; 



        for (int i = 0; i < activeBoxes0.Count; i++)
        {
            // set cube ID to enable marker to report which cube has been selected, and give reference 
            CubeMarker cubeMarker0 = activeBoxes0[i].transform.GetComponent<CubeMarker>();
            cubeMarker0.InitializeMarker(i, 0);

            CubeMarker cubeMarker1 = activeBoxes1[i].transform.GetComponent<CubeMarker>();
            cubeMarker1.InitializeMarker(i, 1);
        }
    }

    public void CheckIfTaskIsComplete (int _cubeId, int _playerId, bool _state)
    {
        // set bit corresponding to updated cube to new value
        if (0 == _playerId)
        {
            markerStates0 = (markerStates0 & ~(1 << _cubeId)) | ((Convert.ToInt32(_state) & 1) << _cubeId);
        }
        else if (1 == _playerId)
        {
            markerStates1 = (markerStates1 & ~(1 << _cubeId)) | ((Convert.ToInt32(_state) & 1) << _cubeId);
        }

        // check if it aligns with desired value
        if (targetMarkerState == markerStates0 && targetMarkerState == markerStates1)
        {
            Debug.Log("Task complete!");
            experimentRunner.NotifyServerToAdvanceExperimentServerRpc();
        }
    }


}
