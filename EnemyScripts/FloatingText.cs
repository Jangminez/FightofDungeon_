using UnityEngine;

public class FloatingText : MonoBehaviour
{
    float _destroyTime = 1f;
    Vector3 _offset = new Vector3(0, 1f, 0);

    void Start() 
    {
        Destroy(gameObject, _destroyTime);

        transform.localPosition += _offset;
    }
}
