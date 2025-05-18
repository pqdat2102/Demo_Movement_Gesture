using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXControllerV3D : MonoBehaviour
{

    public AudioSource loopingSFX;
    public GameObject[] waveSfxPrefabs;

    private float globalProgress;
    private bool _isAttack;
    private bool _isStartAttack;

    public void Attack(bool state)
    {
        _isAttack = state;
        if (_isAttack) _isStartAttack = true;
    }
    public bool AttackStart()
    {
        if (_isStartAttack)
        {
            _isStartAttack = false;
            return true;
        }

        return false;
    }

    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(waveSfxPrefabs[Random.Range(0, waveSfxPrefabs.Length)], transform.position, transform.rotation);
        }

        loopingSFX.volume = globalProgress;
    }
}
