using UnityEngine;
using System.Collections;
public class AudioTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayExplosion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayExplosion()
    {
        while (true)
        {
            yield return new WaitForSeconds(4.0f);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.explosion, transform.position);
        }
    }
}
