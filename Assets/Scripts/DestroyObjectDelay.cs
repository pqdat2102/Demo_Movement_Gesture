using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectDelay : MonoBehaviour
{
    public float destroyTime;
    public void Awake()
    {
        StartCoroutine(DestroyAfter(destroyTime));
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}
