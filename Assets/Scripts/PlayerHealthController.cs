using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public float _health = 100f;
    public void DealDamage(float damage)
    {
        _health -= 100f;
    }    
}
