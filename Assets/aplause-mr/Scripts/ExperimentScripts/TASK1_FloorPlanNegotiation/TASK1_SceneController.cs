using System.Collections;
using UnityEngine;

public class TASK1_SceneController : APMR_SceneController
{


    public GameObject floorPlanDisplayGameObject;
    private MeshRenderer floorPlanDisplayMeshRenderer;


    [SerializeField]
    private bool showSurroundingEnvironments = false;

    enum NumberOfAssignableRooms
    {
        FourRooms,
        FiveRooms
    };

    [SerializeField]
    private NumberOfAssignableRooms numberOfAssignableRooms = NumberOfAssignableRooms.FourRooms;

    protected override void Start()
    {
        base.Start();
        floorPlanDisplayMeshRenderer = floorPlanDisplayGameObject.GetComponent<MeshRenderer>();
    }

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        string numRoomsString = (NumberOfAssignableRooms.FourRooms == numberOfAssignableRooms ? "4" : "5") + "bed";
        string showEnvironmentString = (showSurroundingEnvironments ? "env" : "");
        string floorPlanToLoadString = (trialIndex + 1).ToString();

        // TODO switch when new floor plans are available
        string floorPlanResourcePath = "TASK1/FloorPlanMaterials/plan" + floorPlanToLoadString + "_s3";
        //string floorPlanResourcePath = "TASK1/FloorPlanMaterials/plan" + floorPlanToLoadString + "_" + numRoomsString + "_" + showEnvironmentString;

        Material loadedFloorPlanMaterial = (Material)Resources.Load(floorPlanResourcePath, typeof(Material));
        if (loadedFloorPlanMaterial == null)
        {
            Debug.LogError("floor plan at resource path " + floorPlanResourcePath + " not found");
        }
        floorPlanDisplayMeshRenderer.material = loadedFloorPlanMaterial;
        floorPlanDisplayGameObject.SetActive(true);
    }

    public override void ClearScene()
    {
        base.ClearScene();
        floorPlanDisplayGameObject.SetActive(false);

    }

}
