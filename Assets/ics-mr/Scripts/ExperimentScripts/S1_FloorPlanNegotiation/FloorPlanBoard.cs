using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanBoard : MonoBehaviour
{
    public MeshRenderer floorPlanGeometryMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (floorPlanGeometryMeshRenderer == null)
        {
            floorPlanGeometryMeshRenderer = GameObject.Find("FloorPlanGeometry").GetComponent<MeshRenderer>();
        }
    }


    public void LoadFloorPlan(int floorPlanToLoad)
    {
        Material loadedFloorPlanMaterial = (Material)Resources.Load("FloorPlans/Materials/plan" + floorPlanToLoad, typeof(Material));
        if (loadedFloorPlanMaterial == null)
        {
            Debug.LogError("floor plan not found");
        }
        floorPlanGeometryMeshRenderer.material = loadedFloorPlanMaterial;
    }
}
