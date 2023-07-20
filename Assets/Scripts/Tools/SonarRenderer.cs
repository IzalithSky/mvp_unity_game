using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SonarRenderer : MonoBehaviour
{
    public bool autoScale = false;

    public SonarTracker sonarTracker;
    public MarkersState markersState;
    public int currentSonarIndex = 0;
    
    public string cameraTag = "Marker Camera";
    public float defaultScale = 4f;
    public float scaleMultipliyer = 1f;

    public List<string> detectableTags;
    public List<GameObject> markerPrefabs;
    
    public float detectRadius = 100f;
    public float detectInterval = 2.5f;
    public float markerDuration = 2.3f;
    
    Camera c;
    Dictionary<string, GameObject> tagToPrefab;

    HashSet<Sonar> processedSonars = new HashSet<Sonar>();


    void Awake()
    {        
        tagToPrefab = new Dictionary<string, GameObject>();

        for (int i = 0; i < Mathf.Min(detectableTags.Count, markerPrefabs.Count); i++)
        {
            tagToPrefab[detectableTags[i]] = markerPrefabs[i];
        }

        UpdateSonars();
    }

    void UpdateSonars() {
        foreach (Sonar s in sonarTracker.sonars)
        {
            if (!processedSonars.Contains(s))
            {
                StartCoroutine(DetectAndMark(s));
                processedSonars.Add(s);
            }
        }
    }

    IEnumerator DetectAndMark(Sonar s)
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(s.transform.position, detectRadius);

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

    void Update() {
        UpdateSonars();

        if (!c) {
            GameObject[] cameraGameObjects = GameObject.FindGameObjectsWithTag(cameraTag);
            foreach (GameObject cameraGameObject in cameraGameObjects) {
                Camera possibleCamera = cameraGameObject.GetComponent<Camera>();
                if (possibleCamera && possibleCamera.enabled) {
                    c = possibleCamera;
                    break;
                }
            }
        }
        
        if (c) {
            foreach (Marker m in markersState.AllMarkers) {
                m.transform.LookAt(c.transform);

                if (autoScale) {
                    float distance = Vector3.Distance(transform.position, c.transform.position);
                    float scale = distance * scaleMultipliyer;
                    m.transform.localScale = new Vector3(scale, scale, scale);
                } else {
                    m.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
                }
            }
        }
    }
}
