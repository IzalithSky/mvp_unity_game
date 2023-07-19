using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarkerRenderer : MonoBehaviour
{
    public bool autoScale = false;

    public List<Sonar> sonars;
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
    List<Marker> allMarkers = new List<Marker>();
    Dictionary<string, GameObject> tagToPrefab;


    void GetSonars() {
        ;
    }

    void Awake()
    {
        tagToPrefab = new Dictionary<string, GameObject>();

        for (int i = 0; i < Mathf.Min(detectableTags.Count, markerPrefabs.Count); i++)
        {
            tagToPrefab[detectableTags[i]] = markerPrefabs[i];
        }

        foreach (Sonar s in sonars) {
            StartCoroutine(DetectAndMark(s));
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
                    allMarkers.Add(marker.GetComponent<Marker>());
                    Destroy(marker, markerDuration);
                }
            }

            yield return new WaitForSeconds(detectInterval);
        }
    }

    public void CleanUpDeadSonars()
    {
        sonars.RemoveAll(sonar => sonar == null);
    }

    void Update() {
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
            foreach (Marker m in allMarkers) {
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
