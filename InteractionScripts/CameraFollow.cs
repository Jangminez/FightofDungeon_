using UnityEngine;

//let camera follow target
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed = 1.0f;

    public Vector3 offset;
    private Vector3 targetPos;
    
    private void Update()
    {
        if (target == null) return;

        targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }

}