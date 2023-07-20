using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public MarkersState markersState;

    private void Awake()
    {
        markersState.AllMarkers.Add(this);
    }

    private void OnDestroy()
    {
        markersState.AllMarkers.Remove(this);
    }
}
