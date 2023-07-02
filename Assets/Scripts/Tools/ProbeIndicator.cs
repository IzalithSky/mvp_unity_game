using UnityEngine;
using UnityEngine.Events;

public class ProbeIndicator : MonoBehaviour
{
    public Transform targetTransform;
    private ProbeLauncher probeLauncher;
    
    public UnityEvent<GameObject> OnDestroyed;

    private void Awake()
    {
        GameObject extractionObject = GameObject.Find("Extraction");
        if(extractionObject != null) {
            targetTransform = extractionObject.transform;
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
        if (targetTransform != null)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            // A sphere's diameter is twice the radius. The distance to the target is used as the diameter.
            // Therefore, the scale of the sphere is set to the distance in all three dimensions to achieve the desired effect.
            transform.localScale = Vector3.one * distance * 2;
        }
    }
}
