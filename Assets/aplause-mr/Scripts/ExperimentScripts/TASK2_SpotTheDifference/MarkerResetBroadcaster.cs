using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerResetBroadcaster : MonoBehaviour
{

    // We use this to broadcast
    public event BroadcastHandler Reset;

    // These are the arguments that are broadcast (in our case they're empty)
    public EventArgs e = null;

    // Instantiate the EventHandler
    public delegate void BroadcastHandler(MarkerResetBroadcaster b, EventArgs e);

    public void SendResetMessageToMarkers()
    {
        Reset(this, e);
    }
}
