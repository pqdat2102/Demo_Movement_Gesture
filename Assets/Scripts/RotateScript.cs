using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public Vector3 targetEulerAngles;  // Rotation đích (A) dạng Euler
    public float duration = 2f;        // Thời gian T để xoay

    private Quaternion startRotation;
    private Quaternion endRotation;
    private float timer = 0f;
    private bool rotating = false;

    void Start()
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(targetEulerAngles);
        rotating = true;
    }

    void Update()
    {
        if (!rotating) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

        if (t >= 1f)
        {
            rotating = false; // Hoàn thành
        }
    }
}
