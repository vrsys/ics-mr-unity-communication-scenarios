using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CubeMarker : NetworkBehaviour
{
    [HideInInspector]
    //public bool markerVisible;

    private Transform markerTransform;

    private Vector3 markerVisibleLocalPosition;
    private Vector3 markerHiddenLocalPosition;

    private int cubeId;
    private int playerId;
    private DifferenceObjectController diffObjectsController;

    private float timeOfLastTrigger;


    [HideInInspector]
    public NetworkVariable<bool> isMarked = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    void Start()
    {
        //markerVisible = false;

        markerTransform = gameObject.transform.Find("Marker");
        if (markerTransform == null)
        {
            Debug.Log("No marker found for cube: " + GetPath(gameObject.transform));
        }

        //MarkerResetBroadcaster markerResetBroadcaster = FindAnyObjectByType<MarkerResetBroadcaster>();
        //markerResetBroadcaster.Reset += new MarkerResetBroadcaster.BroadcastHandler(ResetMarker);

        diffObjectsController = FindAnyObjectByType<DifferenceObjectController>();

        if (null == diffObjectsController)
        {
            Debug.LogError("no controller found");
        }
        //if (null == markerResetBroadcaster)
        //{
        //    Debug.LogError("no reset broadcaster found");
        //}


        markerVisibleLocalPosition = markerTransform.localPosition;
        markerHiddenLocalPosition = markerVisibleLocalPosition + new Vector3(0f,0f,0.1f);

        timeOfLastTrigger = Time.time;

    }

    public void ResetMarker(int _cubeId, int _playerId)
    {
        cubeId = _cubeId;
        playerId = _playerId;
        UpdateMarkerStateRpc(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        float timeSinceAcceptedTrigger = Time.time - timeOfLastTrigger;
        if (timeSinceAcceptedTrigger < 0.2f)
        {
            return;
        }

        UpdateMarkerStateRpc(!isMarked.Value);

        timeOfLastTrigger = Time.time;
    }


    [Rpc(SendTo.Server)]
    public void UpdateMarkerStateRpc(bool newState)
    {
        isMarked.Value = newState;
    }

    public static string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }



    private void Update()
    {
        if (isMarked.Value)
        {
            markerTransform.localPosition = markerVisibleLocalPosition;
        }
        else
        {
            markerTransform.localPosition = markerHiddenLocalPosition;
        }
    }
}
