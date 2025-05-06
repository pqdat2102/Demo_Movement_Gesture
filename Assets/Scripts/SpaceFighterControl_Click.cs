using UnityEngine;

public class SpaceshipController_Click : MonoBehaviour
{
    // Tốc độ di chuyển
    public float moveSpeed = 20f;
    // Tốc độ xoay
    public float rotationSpeed = 100f;
    // Lực đẩy tăng tốc
    public float boostMultiplier = 2f;
    // Lực tăng tốc về phía trước
    public float forwardBoostForce = 50f;
    // Khoảng cách di chuyển mỗi lần tích checkbox
    public float moveStep = 0.5f;
    // Góc xoay mỗi lần tích checkbox
    public float rotationStep = 5f;

    // Các biến checkbox trong Inspector cho di chuyển
    public bool moveForward = false;
    public bool moveBackward = false;
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool moveUp = false;
    public bool moveDown = false;
    public bool boostForward = false;

    // Các biến checkbox trong Inspector cho xoay
    public bool pitchUp = false;
    public bool pitchDown = false;
    public bool yawLeft = false;
    public bool yawRight = false;
    public bool rollLeft = false;
    public bool rollRight = false;

    private Rigidbody rb;
    // Lưu trạng thái checkbox trước đó để phát hiện thay đổi
    private bool prevMoveForward = false;
    private bool prevMoveBackward = false;
    private bool prevMoveLeft = false;
    private bool prevMoveRight = false;
    private bool prevMoveUp = false;
    private bool prevMoveDown = false;
    private bool prevBoostForward = false;
    private bool prevPitchUp = false;
    private bool prevPitchDown = false;
    private bool prevYawLeft = false;
    private bool prevYawRight = false;
    private bool prevRollLeft = false;
    private bool prevRollRight = false;

    void Start()
    {
        // Lấy component Rigidbody
        rb = GetComponent<Rigidbody>();
        // Đảm bảo Rigidbody không bị ảnh hưởng bởi trọng lực
        rb.useGravity = false;
    }

    void Update()
    {
        // Kiểm tra thay đổi trạng thái checkbox và di chuyển
        if (moveForward && !prevMoveForward)
        {
            MoveInDirection(transform.forward);
        }
        if (moveBackward && !prevMoveBackward)
        {
            MoveInDirection(-transform.forward);
        }
        if (moveLeft && !prevMoveLeft)
        {
            MoveInDirection(-transform.right);
        }
        if (moveRight && !prevMoveRight)
        {
            MoveInDirection(transform.right);
        }
        if (moveUp && !prevMoveUp)
        {
            MoveInDirection(transform.up);
        }
        if (moveDown && !prevMoveDown)
        {
            MoveInDirection(-transform.up);
        }
        if (boostForward && !prevBoostForward)
        {
            BoostForward();
        }

        // Kiểm tra thay đổi trạng thái checkbox và xoay
        if (pitchUp && !prevPitchUp)
        {
            rb.AddTorque(transform.right * rotationStep, ForceMode.Impulse);
        }
        if (pitchDown && !prevPitchDown)
        {
            rb.AddTorque(-transform.right * rotationStep, ForceMode.Impulse);
        }
        if (yawLeft && !prevYawLeft)
        {
            rb.AddTorque(transform.up * rotationStep, ForceMode.Impulse);
        }
        if (yawRight && !prevYawRight)
        {
            rb.AddTorque(-transform.up * rotationStep, ForceMode.Impulse);
        }
        if (rollLeft && !prevRollLeft)
        {
            rb.AddTorque(transform.forward * rotationStep, ForceMode.Impulse);
        }
        if (rollRight && !prevRollRight)
        {
            rb.AddTorque(-transform.forward * rotationStep, ForceMode.Impulse);
        }

        // Cập nhật trạng thái trước đó
        prevMoveForward = moveForward;
        prevMoveBackward = moveBackward;
        prevMoveLeft = moveLeft;
        prevMoveRight = moveRight;
        prevMoveUp = moveUp;
        prevMoveDown = moveDown;
        prevBoostForward = boostForward;
        prevPitchUp = pitchUp;
        prevPitchDown = pitchDown;
        prevYawLeft = yawLeft;
        prevYawRight = yawRight;
        prevRollLeft = rollLeft;
        prevRollRight = rollRight;
    }

    // Hàm di chuyển theo hướng chỉ định
    public void MoveInDirection(Vector3 moveDirection)
    {
        // Chuẩn hóa vector hướng và áp dụng khoảng di chuyển
        moveDirection = moveDirection.normalized;
        rb.AddForce(moveDirection * moveSpeed * moveStep, ForceMode.Impulse);
    }

    // Hàm tăng tốc về phía trước
    public void BoostForward()
    {
        // Thêm lực mạnh về phía trước của tàu
        rb.AddForce(transform.forward * forwardBoostForce, ForceMode.Impulse);
    }
}