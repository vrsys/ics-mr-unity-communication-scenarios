using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMarker : MonoBehaviour
{
    [HideInInspector]
    public bool markerVisible;

    private Transform markerTransform;

    private Vector3 markerVisibleLocalPosition;
    private Vector3 markerHiddenLocalPosition;

    public int id;
    public int shapeSet;
    public bool isDifferenceCube;
    public DifferenceObjectController diffObjectsController;

    private float timeOfLastTrigger;

    // Start is called before the first frame update
    void Start()
    {
        markerVisible = false;
        markerTransform = gameObject.transform.Find("Marker");
        if (markerTransform == null)
        {
            Debug.Log("No marker found for cube: " + GetPath(gameObject.transform));
        }

        MarkerResetBroadcaster markerResetBroadcaster = GameObject.Find("MarkerResetBroadcaster").GetComponent<MarkerResetBroadcaster>();
        markerResetBroadcaster.Reset += new MarkerResetBroadcaster.BroadcastHandler(ResetMarker);

        markerVisibleLocalPosition = markerTransform.localPosition;
        markerHiddenLocalPosition = markerVisibleLocalPosition + new Vector3(0f,0f,0.1f);

        UpdateMarker();

        timeOfLastTrigger = Time.time;

    }

    private void OnTriggerEnter(Collider other)
    {
        float timeSinceAcceptedTrigger = Time.time - timeOfLastTrigger;
        if (timeSinceAcceptedTrigger < 0.2f)
        {
            return;
        }

        bool newState = !markerVisible;

        if (diffObjectsController != null) {
            diffObjectsController.MarkerSet(newState, id, shapeSet);
        } else {
            SetMarkerState(newState);
        }

        timeOfLastTrigger = Time.time;
    }

    public void SetMarkerState(bool state)
    {
        markerVisible = state;
        UpdateMarker();
    }
    
    private void UpdateMarker()
    {
        if (markerVisible)
        {
            markerTransform.localPosition = markerVisibleLocalPosition;
        } else
        {
            markerTransform.localPosition = markerHiddenLocalPosition;
        }
    }
    public static string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }

    private void ResetMarker(MarkerResetBroadcaster b, EventArgs e)
    {
        markerVisible = false;
        UpdateMarker();
    }

}
