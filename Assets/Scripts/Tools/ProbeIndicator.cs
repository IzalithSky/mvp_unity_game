using UnityEngine;

public class ProbeIndicator : MonoBehaviour
{
    public GameObject targetGameObject;

    private void Update()
    {
        if (targetGameObject != null)
        {
            float distance = Vector3.Distance(transform.position, targetGameObject.transform.position);
            transform.localScale = Vector3.one * distance * 2;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
