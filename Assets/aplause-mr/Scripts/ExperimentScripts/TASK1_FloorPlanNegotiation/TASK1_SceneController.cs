using System.Collections;
using UnityEngine;

public class TASK1_SceneController : APMR_SceneController
{


    public GameObject floorPlanDisplayGameObject;
    private MeshRenderer floorPlanDisplayMeshRenderer;


    protected override void Start()
    {
        base.Start();
        floorPlanDisplayMeshRenderer = floorPlanDisplayGameObject.GetComponent<MeshRenderer>();
    }

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        // load floor plan
        int floorPlanToLoad = trialIndex;
        string floorPlanResourcePath = "TASK1/FloorPlanMaterials/plan" + floorPlanToLoad + "_s3";
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
