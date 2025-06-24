using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndGame : MonoBehaviour
{
    public float delayTime;
    public UnityEvent endGameEvent;
    public void Awake()
    {
        StartCoroutine(BackToMenu(delayTime));
    }

    IEnumerator BackToMenu(float time)
    {
        yield return new WaitForSeconds(time);
        endGameEvent.Invoke();
    }
}
