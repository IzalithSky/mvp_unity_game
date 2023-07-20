using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SonarTracker", menuName = "ScriptableObjects/SonarTracker")]
public class SonarTracker : ScriptableObject
{
    public List<Sonar> sonars = new List<Sonar>();
}
