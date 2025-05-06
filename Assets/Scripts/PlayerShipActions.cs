using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerShipActions : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Transform playerTransform;

    [Header("Vehicle Control")]
    [SerializeField] SpaceshipController spaceshipController;
    [Header("Left Hand")]
    [SerializeField] Transform leftHandTransform;

    // Current HandStates
    private delegate void HandState();
    private HandState rightHandState;
    private HandState leftHandState;

    // Set States in Event Handler
    private string _leftHandStateName;
    private string _rightHandStateName;

    //Dash
    public float dashCooldown = 2.0f;
    public float dashCooldownDelta = -1.0f;
    public float dashTime = 0.2f;
    public float dashTimeDelta = 0.2f;

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

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        // if Photonview isnt owned, then dont allow actions
        // if (!photonView.IsMine) return;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
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
    void MoveDirectionPoint()
    {
        spaceshipController.HandleGesture(leftHandTransform, false);
    }

    void DashDitectionPoint()
    {
        spaceshipController.HandleGesture(leftHandTransform, true);
    }

    /// <summary>
    /// Fires the action if the cooldown is not active.
    /// </summary>
    private void Firing()
    {

    }
    private void StopFiring()
    {

    }

    /// <summary>
    /// Handles the cooldown logic.
    /// </summary>
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
            case "MoveDirectionPoint":
                leftHandState = MoveDirectionPoint;
                break;
            case "DashDirectionPoint":
                leftHandState = DashDitectionPoint;
                break;
            default:
                leftHandState = null;
                break;
        }
    }
}

