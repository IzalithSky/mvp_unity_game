using UnityEngine;
using UnityEngine.Events;

public class ProbeIndicator : MonoBehaviour
{
    public GameObject targetGameObject;
    private ProbeLauncher probeLauncher;
    
    public UnityEvent<GameObject> OnDestroyed;

    private void Awake()
    {
        GameObject extractionObject = GameObject.FindWithTag("Capture Point");
        if(extractionObject != null) {
            targetGameObject = extractionObject;
        }

        probeLauncher = GameObject.FindObjectOfType<ProbeLauncher>();

        if (probeLauncher != null)
        {
            probeLauncher.OnNewProbeIndicatorCreated.Invoke(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke(this.gameObject);
        }
    }

    private void Update()
    {
        if (targetGameObject != null)
        {
            float distance = Vector3.Distance(transform.position, targetGameObject.transform.position);
            // A sphere's diameter is twice the radius. The distance to the target is used as the diameter.
            // Therefore, the scale of the sphere is set to the distance in all three dimensions to achieve the desired effect.
            transform.localScale = Vector3.one * distance * 2;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
