using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public SonarTracker sonarTracker;
    
    public List<string> detectableTags;
    public List<GameObject> markerPrefabs;

    private Dictionary<string, GameObject> tagToPrefab;
    public float detectRadius = 100f;
    public float detectInterval = 2.5f;
    public float markerDuration = 2.3f;

    private void Awake()
    {
        sonarTracker.sonars.Add(this);

        tagToPrefab = new Dictionary<string, GameObject>();

        for (int i = 0; i < Mathf.Min(detectableTags.Count, markerPrefabs.Count); i++)
        {
            tagToPrefab[detectableTags[i]] = markerPrefabs[i];
        }

        StartCoroutine(DetectAndMark());
    }

    private void OnDestroy()
    {
        sonarTracker.sonars.Remove(this);
    }

    private IEnumerator DetectAndMark()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                if (tagToPrefab.ContainsKey(hitCollider.tag))
                {
                    GameObject markerPrefab = tagToPrefab[hitCollider.tag];
                    GameObject marker = Instantiate(markerPrefab, hitCollider.transform.position, Quaternion.identity);
                    Destroy(marker, markerDuration);
                }
            }

            yield return new WaitForSeconds(detectInterval);
        }
    }
}