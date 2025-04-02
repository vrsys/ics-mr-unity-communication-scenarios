using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CubeMarker : NetworkBehaviour
{

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

    public override void OnNetworkSpawn()
    {
        isMarked.OnValueChanged += SetMarkerPosition;
    }

    public override void OnNetworkDespawn()
    {
        isMarked.OnValueChanged -= SetMarkerPosition;
    }

    private void SetMarkerPosition(bool previous, bool current)
    {
        Debug.Log("marker position set to " + (isMarked.Value ? "visible" : "not visible"));
        if (isMarked.Value)
        {
            markerTransform.localPosition = markerVisibleLocalPosition;
        }
        else
        {
            markerTransform.localPosition = markerHiddenLocalPosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        markerTransform = gameObject.transform.Find("Marker");
        if (markerTransform == null)
        {
            Debug.Log("No marker found for cube: " + GetPath(gameObject.transform));
        }

        diffObjectsController = FindAnyObjectByType<DifferenceObjectController>();

        if (null == diffObjectsController)
        {
            Debug.LogError("no controller found");
        }

        markerVisibleLocalPosition = markerTransform.localPosition;
        markerHiddenLocalPosition = markerVisibleLocalPosition + new Vector3(0f,0f,0.1f);

        timeOfLastTrigger = Time.time;

        SetMarkerPosition(true, false);

    }

    public void ResetMarker(int _cubeId, int _playerId)
    {
        cubeId = _cubeId;
        playerId = _playerId;
        UpdateMarkerStateRpc(false);
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");

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
        Debug.Log("marker state set to " + (isMarked.Value ? "visible" : "not visible"));


        isMarked.Value = newState;

        // TODO prompt difference object controller to check if task is complete
    }

    public static string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }

}
