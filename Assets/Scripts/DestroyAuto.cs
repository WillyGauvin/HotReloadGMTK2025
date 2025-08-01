using UnityEngine;

public class DestroyAuto : MonoBehaviour
{
    public float destroyTime = 5.0f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
