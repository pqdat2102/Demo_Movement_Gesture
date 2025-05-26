using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterVehicle : MonoBehaviour
{
    public Transform playerOut;
    public GameObject detechHandOut;
    public Transform playerIn;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == playerOut)
        {
            playerOut.gameObject.SetActive(false);
            detechHandOut.gameObject.SetActive(false);
            playerIn.gameObject.SetActive(true);
        }    
    }
}
