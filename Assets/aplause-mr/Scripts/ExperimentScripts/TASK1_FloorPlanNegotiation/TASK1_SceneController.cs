using System.Collections;
using UnityEngine;

public class TASK1_SceneController : APMR_SceneController
{


    public GameObject floorPlanDisplayGameObject;
    private MeshRenderer floorPlanDisplayMeshRenderer;


    [SerializeField]
    private bool showSurroundingEnvironments = false;

    [SerializeField, Range(4, 5)]
    private int numberOfAssignableRooms = 4;

    protected override void Start()
    {
        base.Start();
        floorPlanDisplayMeshRenderer = floorPlanDisplayGameObject.GetComponent<MeshRenderer>();
    }

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        string numRoomsString = numberOfAssignableRooms.ToString();
        string showEnvironmentString = (showSurroundingEnvironments ? "_env" : "");
        string floorPlanToLoadString = (trialIndex + 1).ToString();

        string floorPlanResourcePath = "TASK1/FloorPlanMaterials/" + numRoomsString + "Rooms/plan" + floorPlanToLoadString + "_" + numRoomsString + "room" + showEnvironmentString;

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
