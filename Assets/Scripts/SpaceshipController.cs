using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    // Tốc độ di chuyển
    public float moveSpeed = 20f;
    // Tốc độ xoay/nghiêng
    public float rotationSpeed = 100f;
    // Lực tăng tốc về phía trước
    public float forwardBoostForce = 50f;
    // Ngưỡng góc để nghiêng ngang (roll)
    public float rollAngleThreshold = 60f; // Góc từ 60 độ
    // Ngưỡng góc để nghiêng lên (pitch)
    public float pitchAngleThreshold = 30f; // Góc từ 30 độ so với trục Y

    private Rigidbody rb;

    void Start()
    {
        // Lấy component Rigidbody
        rb = GetComponent<Rigidbody>();
        // Đảm bảo Rigidbody không bị ảnh hưởng bởi trọng lực
        rb.useGravity = false;
    }

    // Hàm chính để xử lý gesture
    public void HandleGesture(Transform handForward, bool boost)
    {
        // Lấy hướng forward của tay trong world space
        Vector3 handForwardDir = handForward.forward;

        // Debug để kiểm tra giá trị
        Debug.Log($"Hand Forward: {handForwardDir}, Ship Forward: {transform.forward}");

        // Di chuyển tàu dựa trên hướng của tàu
        MoveDirectionPoint();

        // Xử lý nghiêng tàu dựa trên hướng tay
        HandleShipRotation(handForwardDir);

        // Xử lý tăng tốc nếu gesture yêu cầu
        if (boost)
        {
            BoostForward();
        }
    }

    // Hàm di chuyển dựa trên hướng của tàu
    private void MoveDirectionPoint()
    {
        // Lấy vector forward của tàu
        Vector3 moveDirection = transform.forward;

        // Chỉ giữ lại thành phần X và Z, đặt Y = 0 để di chuyển trên mặt phẳng XZ
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;

        // Áp dụng lực di chuyển
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
    }

    // Hàm xử lý nghiêng tàu dựa trên góc tay
    private void HandleShipRotation(Vector3 handForward)
    {
        // Tính góc so với trục Y (Vector3.up) để xác định pitch
        float angleToUp = Vector3.Angle(handForward, Vector3.up);

        // Tính toán nghiêng ngang (roll) bằng cách so sánh hướng tay và tàu trên mặt phẳng XZ
        float rollAngle = 0f;
        Vector3 handForwardXZ = new Vector3(handForward.x, 0f, handForward.z).normalized;
        Vector3 shipForwardXZ = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        float angleBetween = Vector3.Angle(handForwardXZ, shipForwardXZ);
        if (angleBetween > rollAngleThreshold)
        {
            Vector3 cross = Vector3.Cross(shipForwardXZ, handForwardXZ);
            // Xác định hướng nghiêng dựa trên dấu của cross.y
            rollAngle = cross.y > 0 ? -rotationSpeed : rotationSpeed;
        }

        // Tính toán nghiêng lên (pitch)
        float pitchAngle = 0f;
        if (angleToUp < pitchAngleThreshold)
        {
            // Tay hướng lên, nghiêng tàu lên (pitch âm)
            pitchAngle = -rotationSpeed;
        }
        else if (angleToUp > 90f && angleToUp < 180f - pitchAngleThreshold)
        {
            // Tay hướng xuống, nghiêng tàu xuống (pitch dương)
            pitchAngle = rotationSpeed;
        }

        // Áp dụng torque để nghiêng tàu
        Vector3 rotationTorque = new Vector3(pitchAngle, 0f, rollAngle) * Time.deltaTime;
        rb.AddTorque(rotationTorque, ForceMode.Force);
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
        rb.AddForce(transform.forward * forwardBoostForce, ForceMode.Impulse);
    }
}