using UnityEngine;
using VSX.Vehicles;
using VSX.Weapons;

public class SpaceshipController : MonoBehaviour
{
    public float moveSpeed => (moveDefaultSpeed + moveBonusSpeed);
    [SerializeField] private float moveDefaultSpeed = 30.0f;
    [SerializeField] private float moveBonusSpeed = 0.0f;
    public void SetMoveBonusSpeed(float speed) => moveBonusSpeed = speed;
   // Tốc độ xoay/nghiêng
    public float rotationSpeed = 150f;
    // Lực tăng tốc về phía trước
    public float forwardBoostForce = 50f;
    // Góc nghiêng tối đa (độ)
    public float maxTiltAngle = 45f;
    // Độ nghiêng pitch khi lên/xuống
    public float pitchIntensity = 0.7f;
    // Ngưỡng góc roll để xác định trái/phải (độ)
    public float rollThreshold = 0.1f;
    // Ngưỡng góc lệch cho yaw nhanh (độ)
    public float yawAngleThreshold = 60f;
    // Hệ số tăng tốc độ yaw khi vượt ngưỡng
    public float yawSpeedMultiplier = 7f;
    public SpaceFighterAimControl spaceFighterAim;

    private Rigidbody rb;

    private Triggerable triggerable;

    void Start()
    {
        // Lấy component Rigidbody
        rb = GetComponent<Rigidbody>();
        // Đảm bảo Rigidbody không bị ảnh hưởng bởi trọng lực
        rb.useGravity = false;

        triggerable = GetComponentInChildren<Triggerable>();
    }

    public void HandleFire(bool state)
    {
        if (triggerable == null) triggerable = GetComponentInChildren<Triggerable>();
        if (state) triggerable.StartTriggering();
        else triggerable.StopTriggering();
    }    

    // Hàm xử lý gesture Thumb Up (pitch lên trong hệ local, không di chuyển, không yaw/roll)
    public void HandleThumbUp(Transform leftHandTransform)
    {
        // Nghiêng lên (pitch âm quanh trục X local)
        Vector3 rotationTorque = new Vector3(-rotationSpeed * yawSpeedMultiplier * pitchIntensity, 0f, 0f) * Time.deltaTime;
        rb.AddTorque(transform.right * rotationTorque.x, ForceMode.Force);

        // Debug
        Debug.Log("Thumb Up: Pitch Up, No Movement");
        DebugShipTilt();
    }

    // Hàm xử lý gesture Thumb Down (pitch xuống trong hệ local, không di chuyển, không yaw/roll)
    public void HandleThumbDown(Transform leftHandTransform)
    {
        // Nghiêng xuống (pitch dương quanh trục X local)
        Vector3 rotationTorque = new Vector3(rotationSpeed * yawSpeedMultiplier * pitchIntensity, 0f, 0f) * Time.deltaTime;
        rb.AddTorque(transform.right * rotationTorque.x, ForceMode.Force);

        // Debug
        Debug.Log("Thumb Down: Pitch Down, No Movement");
        DebugShipTilt();
    }

    // Hàm xử lý gesture Hand Point (di chuyển theo XZ local, yaw quanh Y local với tốc độ tăng, không roll/pitch)
    public void HandlePoint(Transform leftHandTransform)
    {
        // Lấy hướng tay và tàu
        Vector3 handForwardDir = leftHandTransform.forward.normalized;
        Vector3 shipForward = transform.forward.normalized;

        // Chuyển hướng tay sang hệ tọa độ tàu
        Vector3 handForwardInShip = transform.InverseTransformDirection(handForwardDir);
        Vector3 handForwardInShipXZ = new Vector3(handForwardInShip.x, 0f, handForwardInShip.z).normalized;

        // Tính góc lệch 3D giữa tàu và tay
        float angleBetween = Vector3.Angle(shipForward, handForwardDir);
        // Tính hệ số tốc độ (cosin để mượt)
        float speedFactor = Mathf.Max(0.1f, Mathf.Cos(angleBetween * Mathf.Deg2Rad)); // Tối thiểu 10% tốc độ
        float adjustedSpeed = moveSpeed * speedFactor;

        // Xoay (yaw quanh Y local, tăng tốc nếu góc lệch lớn)
        float yawSpeed = angleBetween >= yawAngleThreshold ? rotationSpeed * yawSpeedMultiplier : rotationSpeed;
        float yawAngle = Mathf.Clamp(Vector3.SignedAngle(shipForward, handForwardDir, transform.up), -yawSpeed * Time.deltaTime, yawSpeed * Time.deltaTime);
        rb.AddTorque(transform.up * yawAngle, ForceMode.Force);

        // Di chuyển theo XZ local (thẳng hoặc trái/phải)
        float forwardComponent = handForwardInShipXZ.z; // Thành phần forward local
        float rightComponent = handForwardInShipXZ.x;   // Thành phần right local
        
        if (angleBetween <= 15f || spaceFighterAim.IsAiming)
        {
            rightComponent = 0f;
        }

        Vector3 moveDirection = transform.forward * forwardComponent + transform.right * rightComponent;
        rb.AddForce(moveDirection * adjustedSpeed, ForceMode.Force);

        // Debug
        Debug.Log($"Point: Moving to {moveDirection}, Speed Factor: {speedFactor:F2}, Angle Between: {angleBetween:F2}, Yaw Angle: {yawAngle:F2}, Yaw Speed: {yawSpeed:F2}");
        DebugShipTilt();
    }

    // Hàm xử lý gesture Hand Palm (roll trái/phải quanh Z local dựa trên trục Y local của tay, không di chuyển)
    public void HandlePalm(Transform leftHandTransform)
    {
        // Chuyển trục Y local của tay (up) sang hệ tọa độ tàu
        Vector3 handUpInShip = transform.InverseTransformDirection(leftHandTransform.up);
        float xComponent = handUpInShip.x; // Thành phần right local

        // Xác định hướng roll
        float rollAngle = 0f;
        if (xComponent > rollThreshold)
        {
            // Trục Y nghiêng trái (theo tàu) → Roll trái
            rollAngle = rotationSpeed * yawSpeedMultiplier;
            Debug.Log($"Palm: Roll Left (X Component: {xComponent:F2})");
        }
        else if (xComponent < -rollThreshold)
        {
            // Trục Y nghiêng phải → Roll phải
            rollAngle = -rotationSpeed * yawSpeedMultiplier;
            Debug.Log($"Palm: Roll Right (X Component: {xComponent:F2})");
        }
        else
        {
            // X component quá nhỏ
            Debug.Log($"Palm: No Roll (X Component: {xComponent:F2}, below threshold)");
            return;
        }

        // Áp dụng roll quanh trục Z local
        rb.AddTorque(transform.forward * rollAngle * Time.deltaTime, ForceMode.Force);

        // Debug
        DebugShipTilt();
    }

    // Hàm debug góc nghiêng của tàu
    private void DebugShipTilt()
    {
        // Tính góc roll (quanh trục Z) và pitch (quanh trục X)
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float roll = eulerAngles.z;
        float pitch = eulerAngles.x;

        // Chuẩn hóa góc về [-180, 180]
        if (roll > 180f) roll -= 360f;
        if (pitch > 180f) pitch -= 360f;

        Debug.Log($"Ship Tilt - Roll: {roll:F2} degrees, Pitch: {pitch:F2} degrees");
    }

    // Hàm di chuyển theo hướng chỉ định
    public void MoveInDirection(Vector3 moveDirection)
    {
        // Chuẩn hóa vector hướng và áp dụng lực
        moveDirection = moveDirection.normalized;
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
    }

    // Hàm tăng tốc về phía trước
    public void BoostForward()
    {
        // Thêm lực mạnh về phía trước của tàu
        rb.AddTorque(transform.forward * forwardBoostForce, ForceMode.Force);
        Debug.Log("Boost Forward");
        DebugShipTilt();
    }
}