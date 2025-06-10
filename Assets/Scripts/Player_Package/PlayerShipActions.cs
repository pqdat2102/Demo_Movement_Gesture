using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerShipActions : MonoBehaviour
{
    [Header("Vehicle Control")]
    [SerializeField] SpaceshipController spaceshipController;
    [Header("Left Hand")]
    [SerializeField] Transform leftHandPoke;
    [SerializeField] Transform leftHandDirect;

    // Current HandStates
    private delegate void HandState();
    private HandState rightHandState;
    private HandState leftHandState;

    // Set States in Event Handler
    private string _leftHandStateName;
    private string _rightHandStateName;

    //Dash
    [Header("Dash")]
    public float dashCooldownDefault = 2.0f;
    public float dashCooldown => dashCooldownDefault + cooldownBonus;
    public float dashCooldownDelta = -1.0f;
    public float dashTime = 0.2f;
    public float dashTimeDelta = 0.2f;
    public float cooldownBonus = 0.0f;
    public void SetCooldownBonus(float bonus) => cooldownBonus = bonus;
    public string rightHandStateName
    {
        get => _rightHandStateName;
        set
        {
            _rightHandStateName = value;
            HandleStates();
        }
    }
    public string leftHandStateName
    {
        get => _leftHandStateName;
        set
        {
            _leftHandStateName = value;
            HandleStates();
        }
    }

    // Cooldown
    private float count = 0f;
    private bool cooldown = false;
    private const float maxCount = 300f;

    private void Update()
    {
        HandleCooldown();
        HandleDashCooldown();

        rightHandState?.Invoke();
        leftHandState?.Invoke();
    }

    /// <summary>
    /// Moves the player in the direction of the left hand transform.
    /// </summary>
    void PointDirectionPoint()
    {
        spaceshipController.HandlePoint(leftHandPoke);
    } 

    void PalmDirectionPoint()
    {
        spaceshipController.HandlePalm(leftHandPoke);
    }

    void ThumbUpDirection()
    {
        spaceshipController.HandleThumbUp(leftHandDirect);
    }

    void ThumbDownDirection()
    {
        spaceshipController.HandleThumbDown(leftHandDirect);
    }

    private void Firing()
    {
        spaceshipController.HandleFire(true);
    }
    private void StopFiring()
    {
        spaceshipController.HandleFire(false);
    }

    private void HandleCooldown()
    {
        if (count >= maxCount)
        {
            cooldown = true;
        }

        if (count != 0f && cooldown)
        {
            count -= 1f;
        }

        if (count <= 0)
        {
            count = 0;
            cooldown = false;
        }
    }

    private void HandleDashCooldown()
    {
        if (dashCooldownDelta > 0)
        {
            dashCooldownDelta -= Time.deltaTime;
        }
    }

    private void HandleStates()
    {
        switch (rightHandStateName)
        {
            case "Firing":
                rightHandState = Firing;
                break;
            case "StopFiring":
                rightHandState = StopFiring;
                break;
            default:
                rightHandState = null;
                break;
        }

        switch (leftHandStateName)
        {
            case "PointDirectionPoint":
                leftHandState = PointDirectionPoint;
                break;
            case "PalmDirectionPoint":
                leftHandState = PalmDirectionPoint;
                break;
            case "ThumbUpDirection":
                leftHandState = ThumbUpDirection;
                break;
            case "ThumbDownDirection":
                leftHandState = ThumbDownDirection;
                break;
            default:
                leftHandState = null;
                break;
        }
    }
}

