using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerAttackController : MonoBehaviour
{
    public SFXControllerV3D sfgx;
    public ProgressControlV3D progress;
    public Transform TargetObject;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            sfgx.Attack(true);
            progress.Attack(true);
        }    

        if (Input.GetMouseButtonUp(0))
        {
            sfgx.Attack(false);
            progress.Attack(false);
        }    
    }
}
