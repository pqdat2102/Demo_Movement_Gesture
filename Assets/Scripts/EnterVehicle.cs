using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterVehicle : MonoBehaviour
{
    public Transform playerOut;
    public Transform playerIn;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == playerOut)
        {
            playerIn.gameObject.SetActive(true);
            playerOut.gameObject.SetActive(false);
        }    
    }
}
