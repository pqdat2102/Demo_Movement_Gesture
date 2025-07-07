using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWayPointMove : MonoBehaviour
{
    [Header("Waypoints cần đi qua")]
    public List<Transform> waypoints;

    [Header("Tốc độ di chuyển (m/s)")]
    public float moveSpeed = 2f;

    private int currentIndex = 0;
    private bool isMoving = false;

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 endPos;
    private Quaternion endRot;

    private float journeyLength;
    private float journeyTime;
    private float startTime;

    void Start()
    {
        if (waypoints.Count >= 2)
        {
            BeginMove();
        }
    }

    void BeginMove()
    {
        isMoving = true;
        currentIndex = 0;
        SetupNextWaypoint();
    }

    void SetupNextWaypoint()
    {
        if (currentIndex >= waypoints.Count - 1)
        {
            isMoving = false;
            return;
        }

        startPos = waypoints[currentIndex].position;
        endPos = waypoints[currentIndex + 1].position;

        startRot = waypoints[currentIndex].rotation;
        endRot = waypoints[currentIndex + 1].rotation;

        journeyLength = Vector3.Distance(startPos, endPos);
        journeyTime = journeyLength / moveSpeed;
        startTime = Time.time;

        currentIndex++;
    }

    void Update()
    {
        if (!isMoving) return;

        float elapsed = Time.time - startTime;
        float t = Mathf.Clamp01(elapsed / journeyTime);

        transform.position = Vector3.Lerp(startPos, endPos, t);
        transform.rotation = Quaternion.Slerp(startRot, endRot, t);

        if (t >= 1f)
        {
            SetupNextWaypoint();
        }
    }

    public void Restart()
    {
        if (waypoints.Count >= 2)
        {
            BeginMove();
        }
    }
}
