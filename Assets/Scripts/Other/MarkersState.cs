
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkersState", menuName = "ScriptableObjects/MarkersState", order = 2)]
public class MarkersState : ScriptableObject
{
    public List<Marker> AllMarkers = new List<Marker>();
    public bool autoScale = true;
    
    public Transform targetCamera;
}