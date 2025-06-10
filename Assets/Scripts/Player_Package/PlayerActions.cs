using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerActions : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Transform playerTransform;
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] float playerBonusSpeed = 0.0f;
    public void SetPlayerBonusSpeed(float bonus) => playerBonusSpeed = bonus;

    [Header("Left Hand")]
    [SerializeField] Transform leftHandTransform;
    [Header("Left Hand Direct")]
    [SerializeField] Transform leftHandDirectTransform;
    [Header("Right Hand")]
    [SerializeField] GameObject modelShow;
    [Header("Both Hands")]
    [SerializeField] Material handMaterial;
    [SerializeField] GameObject ShieldModel;
    [Header("Teleport")]
    [SerializeField] private LayerMask teleportLayer; // Layer Teleport
    [SerializeField] private float maxDistance = 100f; // Khoảng cách tối đa của tia
    [SerializeField] private GameObject teleportLocation; // GameObject teleportLocation
    [SerializeField] private GameObject teleportParticle; // GameObject teleportParticle
    [SerializeField] private float particleDuration = 2f; // Thời gian particle hoạt động (giây)

    // Current HandStates
    private delegate void HandState();
    private HandState rightHandState;
    private HandState leftHandState;

    // Set States in Event Handler
    private string _leftHandStateName;
    private string _rightHandStateName;

    [Space(10)]
    //Dash
    [Header("Dash")]
    public float dashCooldownDefault = 2.0f;
    public float dashCooldownDelta = -1.0f;
    public float dashTime = 0.2f;
    public float dashTimeDelta = 0.2f;
    public float dashCooldown => (dashCooldownDefault + cooldownBonus);
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
        // Lấy vector forward từ leftHandTransform
        Vector3 moveDirection = leftHandTransform.forward * (playerSpeed + playerBonusSpeed) * Time.deltaTime;

        // Chỉ giữ lại thành phần X và Z, đặt Y = 0
        moveDirection.y = 0f;

        // Di chuyển playerTransform chỉ trên trục X và Z
        playerTransform.position += moveDirection;
    }

    void DashDitectionPoint()
    {
        if (dashCooldownDelta < 0 && dashTimeDelta > 0)
        {

            // Lấy vector forward từ leftHandTransform
            Vector3 moveDirection = leftHandTransform.forward * 20.0f * Time.deltaTime;

            // Chỉ giữ lại thành phần X và Z, đặt Y = 0
            moveDirection.y = 0f;

            // Di chuyển playerTransform chỉ trên trục X và Z
            playerTransform.position += moveDirection;

            dashTimeDelta -= Time.deltaTime;

            if (dashTimeDelta < 0)
            {
                dashCooldownDelta = dashCooldown;
                dashTimeDelta = dashTime;
            }
        }
    }

    /// <summary>
    /// Fires the action if the cooldown is not active.
    /// </summary>
    private void Firing()
    {
        if (!cooldown)
        {
            modelShow.GetComponent<Unity.FPS.Game.WeaponController>().HandleShootInputs(false, true, false);
            handMaterial.color = new Color(1f, 1f, 1f, 0f);
            count += 1f;
            modelShow.SetActive(true);
        }
        else
        {
            handMaterial.color = Color.red;
            modelShow.SetActive(false);
        }
    }

    private void StopFiring()
    {
        handMaterial.color = new Color(1f, 1f, 1f, 1f);
        count -= 1f;
        modelShow.SetActive(false);
    }

    private void Shield()
    {
        ShieldModel.SetActive(true);
    }   
    
    private void UnShield()
    {
        ShieldModel.SetActive(false);
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

    void Teleport_Aim()
    {
        teleportLocation.SetActive(true);
        if (leftHandDirectTransform == null || teleportLocation == null)
        {
            Debug.LogWarning("leftHandDirectTransform hoặc teleportLocation chưa được gán!");
            return;
        }

        // Chiếu tia từ vị trí và hướng của leftHandDirectTransform
        Ray ray = new Ray(leftHandDirectTransform.position, leftHandDirectTransform.forward);
        RaycastHit hit;

        // Kiểm tra va chạm với các layer Default hoặc Teleport
        if (Physics.Raycast(ray, out hit, maxDistance, LayerMask.GetMask("Default", "Teleport")))
        {
            // Kiểm tra nếu đối tượng trúng là trigger và có layer Teleport
            if (hit.collider.isTrigger && hit.collider.gameObject.layer == LayerMask.NameToLayer("Teleport"))
            {
                // Đặt teleportLocation đến vị trí điểm trúng
                teleportLocation.transform.position = hit.point;
                Debug.Log($"TeleportLocation đặt tại: {hit.point} (Trigger Teleport)");
            }
            // Kiểm tra nếu trúng layer Default
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                // Đặt teleportLocation đến vị trí điểm trúng
                teleportLocation.transform.position = hit.point;
                Debug.Log($"TeleportLocation đặt tại: {hit.point} (Default Layer)");
            }
        }
    }

    public void Teleport_Start()
    {
        leftHandStateName = "default";
        teleportLocation.SetActive(false);

        if (teleportLocation == null)
        {
            Debug.LogWarning("teleportLocation không tồn tại!");
            return;
        }
        if (teleportParticle == null)
        {
            Debug.LogWarning("teleportParticle không tồn tại!");
            return;
        }

        transform.position = teleportLocation.transform.position;

        // Kích hoạt particle và lên lịch tắt sau 2 giây
        teleportParticle.SetActive(true);
        teleportParticle.GetComponent<SmartWaveParticlesControllerV3D>().SetGlobalProgress(1.0f);
        Invoke(nameof(DeactivateParticle), particleDuration);
    }

    private void DeactivateParticle()
    {
        // Tắt particle sau khi hết thời gian
        if (teleportParticle != null)
        {
            teleportParticle.SetActive(false);
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
            case "Shield":
                rightHandState = Shield;
                break;
            case "UnShield":
                rightHandState = UnShield;
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
            case "Teleport_Aim":
                leftHandState = Teleport_Aim;
                break;
            case "Teleport_Start":
                leftHandState = Teleport_Start;
                break;
            default:
                leftHandState = null;
                break;
        }
    }
}

