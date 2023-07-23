using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ZoneTracker", menuName = "ScriptableObjects/ZoneTracker")]
public class ZoneTracker : ScriptableObject
{
    public List<CaptureZone> zones = new List<CaptureZone>();

    public List<CaptureZone> GetZonesWithTag(string tag)
    {
        return zones.Where(zone => zone.CompareTag(tag)).ToList();
    }
}