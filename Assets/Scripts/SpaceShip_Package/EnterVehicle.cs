using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterVehicle : MonoBehaviour
{
    public Transform playerOut;
    public GameObject detechHandOut;
    public Transform playerIn;
    public LayerMask playerLayer;
    public UnityEvent NearSpaceShipEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player_Hand"))
        {
            NearSpaceShipEvent.Invoke();
        }       
    }
    public void SetEnterVehicle()
    {
        playerOut.gameObject.SetActive(false);
        detechHandOut.gameObject.SetActive(false);
        playerIn.gameObject.SetActive(true);
    }    
}
